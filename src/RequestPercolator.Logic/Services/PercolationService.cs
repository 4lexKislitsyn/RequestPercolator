using Microsoft.AspNetCore.Http;
using RequestPercolator.Model;
using RequestPercolator.Model.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Logic.Services
{
    internal sealed class PercolationService : IPercolationService
    {
        public Task<PercolationResult> PercolateRequestAsync(HttpRequest request, Guid filterId, CancellationToken cancellationToken)
        {
            return Task.FromResult(PercolationResult.ShouldBeReversed);
        }

        public Task<PercolationResult> PercolateRequestAsync(HttpRequest request, string filter, CancellationToken cancellationToken)
        {
            return Task.FromResult(PercolationResult.ShouldBeSkipped);
        }
    }
}
