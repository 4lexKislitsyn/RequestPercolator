using Microsoft.AspNetCore.Mvc;
using Microsoft.ReverseProxy.Service.Proxy;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RequestPercolator.Model.Exceptions;

namespace RequestPercolator.API.Proxy
{
    public sealed class ReversedProxyResult : ActionResult
    {
        private readonly string destinationAddress;

        public ReversedProxyResult(string destinationAddress)
        {
            this.destinationAddress = destinationAddress;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var proxy = context.HttpContext.RequestServices.GetRequiredService<IHttpProxy>();
            var container = context.HttpContext.RequestServices.GetRequiredService<ProxyHandlerContainer>();
            await proxy.ProxyAsync(context.HttpContext, destinationAddress, container.HttpClient, container.ProxyOptions);

            var errorFeature = context.HttpContext.Features.Get<IProxyErrorFeature>();
            if (errorFeature != null)
            {
                throw new ProxyErrorException(errorFeature.Exception, errorFeature.Error.ToString());
            }
        }
    }
}
