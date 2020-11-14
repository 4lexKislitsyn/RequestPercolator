using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Microsoft.ReverseProxy.Service.Proxy;
using Microsoft.ReverseProxy.Service.RuntimeModel.Transforms;
using RequestPercolator.API.Proxy.Transforms;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using RequestTransforms = Microsoft.ReverseProxy.Service.RuntimeModel.Transforms.Transforms;

namespace RequestPercolator.API.Proxy
{
    internal sealed class ProxyHandlerContainer : IDisposable
    {
        public ProxyHandlerContainer(IConfiguration configuration)
        {
            HttpClient = new HttpMessageInvoker(CreateHandler(configuration), true);
            ProxyOptions = CreateProxyOptions(configuration);
        }

        public HttpMessageInvoker HttpClient { get; private set; }

        public RequestProxyOptions ProxyOptions { get; private set; }

        private static SocketsHttpHandler CreateHandler(IConfiguration configuration)
        {
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
            return handler;
        }

        private static RequestProxyOptions CreateProxyOptions(IConfiguration configuration)
        {
            var emptyHeaderTransforms = new Dictionary<string, ResponseHeaderTransform>();
            var proxyOptions = new RequestProxyOptions()
            {
                RequestTimeout = TimeSpan.FromSeconds(100),
                Transforms = new RequestTransforms(
                    copyRequestHeaders: true,
                    requestTransforms: CreateParametersTransforms(),
                    requestHeaderTransforms: CreateRequestHeaderTransforms(),
                    responseHeaderTransforms: emptyHeaderTransforms,
                    responseTrailerTransforms: emptyHeaderTransforms)
            };
            var section = configuration.GetSection(nameof(RequestProxyOptions));
            if (section.Exists())
            {
                section.Bind(proxyOptions);
            }

            return proxyOptions;
        }

        private static Dictionary<string, RequestHeaderTransform> CreateRequestHeaderTransforms()
        {
            return new Dictionary<string, RequestHeaderTransform>()
            {
                { HeaderNames.Host, new RequestHeaderValueTransform(string.Empty, append: false) }
            };
        }

        private static IReadOnlyList<RequestParametersTransform> CreateParametersTransforms()
        {
            return new RequestParametersTransform[]
            {
                new RemovePathTransform(),
                new RemoveQueryParametersByPrefixTransform("_"),
            };
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}
