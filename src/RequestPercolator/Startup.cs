using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hellang.Middleware.ProblemDetails;
using RequestPercolator.Model.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;

namespace RequestPercolator
{
    public class Startup
    {
        private readonly IHostEnvironment environment;
        private readonly IConfiguration configuration;

        public Startup(IHostEnvironment environment, IConfiguration configuration)
        {
            this.environment = environment;
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReverseProxy()
                .LoadFromConfig(configuration);
            services
                .AddPercolatorLogic()
                .AddPercolatorApi()
                .AddPercolatorStorage();
            services.AddControllers();

            services.AddProblemDetails(opt =>
            {
                opt.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();
                MapExceptions(opt);
            });

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.ClientErrorMapping.Clear();
                foreach (int statusCode in Enum.GetValues(typeof(System.Net.HttpStatusCode)))
                {
                    opt.ClientErrorMapping[statusCode] = new ClientErrorData
                    {
                        Link = $"https://httpstatuses.com/{statusCode}",
                        Title = ReasonPhrases.GetReasonPhrase(statusCode),
                    };
                }
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseProblemDetails();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void MapExceptions(ProblemDetailsOptions opt)
        {
            opt.Map<ProxyErrorException>((ctx, ex) =>
            {
                var details = new StatusCodeProblemDetails(ctx.Response.StatusCode)
                {
                    Detail = ex.Message
                };
                details.Extensions["errorType"] = ex.ErrorType;
                details.Extensions["exceptionMessage"] = ex.InnerException?.Message ?? string.Empty;
                return details;
            });
            opt.MapToStatusCode<NotFoundException>(StatusCodes.Status404NotFound);
        }
    }
}
