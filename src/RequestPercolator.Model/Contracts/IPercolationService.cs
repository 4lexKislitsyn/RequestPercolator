using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Model.Contracts
{
    public interface IPercolationService
    {
        Task<PercolationResult> PercolateRequestAsync(HttpRequest request, Guid filterId, CancellationToken cancellationToken);

        Task<PercolationResult> PercolateRequestAsync(HttpRequest request, string filter, CancellationToken cancellationToken);
    }
}
