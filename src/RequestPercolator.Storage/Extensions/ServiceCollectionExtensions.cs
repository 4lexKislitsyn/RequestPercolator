using RequestPercolator.Model.Contracts;
using RequestPercolator.Storage.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPercolatorStorage(this IServiceCollection services)
            => services.AddSingleton<IFilterRepository, InMemoryFilterRepository>();
    }
}
