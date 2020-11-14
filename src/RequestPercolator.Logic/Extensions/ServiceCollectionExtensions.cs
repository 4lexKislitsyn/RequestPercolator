using RequestPercolator.Model.Contracts;
using RequestPercolator.Logic.Services;
using RequestPercolator.Logic.Contracts;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPercolatorLogic(this IServiceCollection services)
        {
            return services.AddScoped<IPercolationService, PercolationService>()
                .AddSingleton<IRequestPercolationHandler, JsonRequestPercolationHandler>()
                .AddSingleton<IRequestPercolationHandler, XmlRequestPercolationHandler>();
        }
    }
}
