using RequestPercolator.API.Proxy;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPercolatorApi(this IServiceCollection services)
            => services.AddSingleton<ProxyHandlerContainer>();
    }
}
