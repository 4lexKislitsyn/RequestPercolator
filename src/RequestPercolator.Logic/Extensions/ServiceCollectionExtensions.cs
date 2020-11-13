using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using RequestPercolator.Model.Contracts;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogic(this IServiceCollection services)
        {
            return services.AddSingleton<IPercolatorProxyService, RequestPercolator.Logic.Services.PercolatorProxyService>();
        }
    }
}
