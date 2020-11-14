using RequestPercolator.Model.Contracts;
using RequestPercolator.Logic.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPercolatorLogic(this IServiceCollection services)
        {
            return services.AddSingleton<IPercolationService, PercolationService>();
        }
    }
}
