using Microsoft.AspNetCore.Http;
using RequestPercolator.Logic.Contracts;
using RequestPercolator.Model;
using RequestPercolator.Model.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Logic.Services
{
    internal sealed class PercolationService : IPercolationService
    {
        private readonly IFilterRepository filterRepository;
        private readonly IEnumerable<IRequestPercolationHandler> requestHandlers;

        public PercolationService(IFilterRepository filterRepository, IEnumerable<IRequestPercolationHandler> requestHandlers)
        {
            this.filterRepository = filterRepository;
            this.requestHandlers = requestHandlers;
        }

        public async Task<PercolationResult> PercolateRequestAsync(HttpRequest request, Guid filterId, CancellationToken cancellationToken)
        {
            var filter = await filterRepository.GetFilterAsync(filterId, cancellationToken);
            return await PercolateRequestAsync(request, filter, cancellationToken);
        }

        public Task<PercolationResult> PercolateRequestAsync(HttpRequest request, string filter, CancellationToken cancellationToken)
        {
            return SelectHandler(request, filter).HandleAsync(request, filter, cancellationToken);
        }

        private IRequestPercolationHandler SelectHandler(HttpRequest request, string filter)
        {
            var candidates = requestHandlers
                .Where(x => x.CanHandleRequest(request)).ToArray();
            if (!candidates.Any())
            {
                throw new NotSupportedException($"Request cannot be handled.");
            }

            return candidates.FirstOrDefault(x => x.CanHandleFilter(filter))
                ?? throw new FormatException($"Expression '{filter}' cannot be handled by any of handlers.");
        }
    }
}
