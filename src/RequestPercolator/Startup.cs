using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestPercolator.Model.Contracts;
using Hellang.Middleware.ProblemDetails;

namespace RequestPercolator
{
    public class Startup
    {
        private readonly IHostEnvironment environment;

        public Startup(IHostEnvironment environment)
        {
            this.environment = environment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReverseProxy();
            services.AddProblemDetails(opt =>
            {
                opt.IncludeExceptionDetails = (ctx, ex) => !environment.IsProduction();
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, IPercolatorProxyService percolatorProxy)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseProblemDetails();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/percolate/{id:guid}", async httpContext =>
                {
                    await percolatorProxy.HandleRequestAsync(httpContext);
                });
            });
        }
    }
}
