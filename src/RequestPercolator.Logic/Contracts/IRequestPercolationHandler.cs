using Microsoft.AspNetCore.Http;
using RequestPercolator.Model;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Logic.Contracts
{
    interface IRequestPercolationHandler
    {
        bool CanHandleRequest(HttpRequest request);

        bool CanHandleFilter(string filter);

        Task<PercolationResult> HandleAsync(HttpRequest request, string filter, CancellationToken cancellationToken);
    }
}
