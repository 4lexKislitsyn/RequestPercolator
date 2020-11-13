using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RequestPercolator.Model.Contracts
{
    public interface IPercolatorProxyService
    {
        Task HandleRequestAsync(HttpContext context);
    }
}
