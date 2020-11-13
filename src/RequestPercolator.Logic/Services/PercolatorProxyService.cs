using System;
using System.Collections.Generic;
using System.Text;
using RequestPercolator.Model.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.ReverseProxy.Service.Proxy;
using System.Net.Http;
using System.Net;
using Microsoft.ReverseProxy.Service.RuntimeModel.Transforms;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Hellang.Middleware.ProblemDetails;
using static System.Net.Mime.MediaTypeNames;

namespace RequestPercolator.Logic.Services
{
    internal sealed class PercolatorProxyService : IPercolatorProxyService
    {
        private readonly IHttpProxy proxy;
        private readonly ILogger logger;
        private readonly HttpMessageInvoker httpClient;
        private readonly RequestProxyOptions proxyOptions;

        public PercolatorProxyService(IHttpProxy proxy,
            ILogger<PercolatorProxyService> logger,
            IConfiguration configuration)
        {
            this.proxy = proxy;
            this.logger = logger;

            var handler = new SocketsHttpHandler
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false
            };

            var section = configuration.GetSection(nameof(SocketsHttpHandler));
            if (section.Exists())
            {
                section.Bind(handler);
            }
            httpClient = new HttpMessageInvoker(handler, true);
            proxyOptions = new RequestProxyOptions()
            {
                RequestTimeout = TimeSpan.FromSeconds(100),
                Transforms = new Transforms(
                    copyRequestHeaders: true,
                    requestTransforms: CreateParametersTransforms(),
                    requestHeaderTransforms: CreateRequestHeaderTransforms(),
                    responseHeaderTransforms: CreateResponseHeaderTransforms(),
                    responseTrailerTransforms: CreateResponseTrailerTransforms())
            };
            section = configuration.GetSection(nameof(RequestProxyOptions));
            if (section.Exists())
            {
                section.Bind(proxyOptions);
            }
        }

        public async Task HandleRequestAsync(HttpContext context)
        {
            if (context.Request.ContentType != Application.Json)
            {
                throw new ProblemDetailsException(StatusCodes.Status415UnsupportedMediaType, $"{context.Request.ContentType} is not supported");
            }

            if (!context.Request.RouteValues.TryGetValue("id", out object filterId))
            {
                throw new ProblemDetailsException(StatusCodes.Status400BadRequest, "Parameter 'id' was not specified");
            }

            if (!(filterId is Guid id) || filterId is string filterIdString && !Guid.TryParse(filterIdString, out id))
            {
                throw new ProblemDetailsException(StatusCodes.Status400BadRequest, "Parameter 'id' was not parsed");
            }

            if (!context.Request.Query.TryGetValue("path", out var proxyAddress))
            {
                throw new ProblemDetailsException(StatusCodes.Status400BadRequest, "Parameter 'path' was not specified");
            }

            if (!Uri.IsWellFormedUriString(proxyAddress, UriKind.Absolute))
            {
                throw new ProblemDetailsException(StatusCodes.Status400BadRequest, "Parameter 'path' is not valid URI");
            }

            await proxy.ProxyAsync(context, proxyAddress, httpClient, proxyOptions);

            var errorFeature = context.Features.Get<IProxyErrorFeature>();
            if (errorFeature != null)
            {
                logger.LogError(errorFeature.Exception, $"{errorFeature.Error} error was occurred on request {context.Request.Path} ({context.Request.QueryString})");
                var details = new StatusCodeProblemDetails(StatusCodes.Status424FailedDependency);
                details.Extensions["errorType"] = errorFeature.Error;
                details.Extensions["exceptionMessage"] = errorFeature.Exception?.Message ?? string.Empty;
                throw new ProblemDetailsException(details);
            }
        }

        private Dictionary<string, RequestHeaderTransform> CreateRequestHeaderTransforms()
        {
            return new Dictionary<string, RequestHeaderTransform>()
            {
                { HeaderNames.Host, new RequestHeaderValueTransform(string.Empty, append: false) }
            };
        }

        private Dictionary<string, ResponseHeaderTransform> CreateResponseHeaderTransforms()
        {
            return new Dictionary<string, ResponseHeaderTransform>();
        }

        private Dictionary<string, ResponseHeaderTransform> CreateResponseTrailerTransforms()
        {
            return new Dictionary<string, ResponseHeaderTransform>();
        }

        private IReadOnlyList<RequestParametersTransform> CreateParametersTransforms()
        {
            return Array.Empty<RequestParametersTransform>();
        }
    }
}
