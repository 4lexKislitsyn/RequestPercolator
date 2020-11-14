using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using RequestPercolator.API.Proxy;
using RequestPercolator.Model.Exceptions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPercolatorApi(this IServiceCollection services)
            => services.AddSingleton<ProxyHandlerContainer>();
    }
}
