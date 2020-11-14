using RequestPercolator.Model.Contracts;
using RequestPercolator.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Storage.Repositories
{
    internal sealed class InMemoryFilterRepository : IFilterRepository
    {
        private readonly Dictionary<Guid, string> filters = new Dictionary<Guid, string>();

        public Task<string> GetFilterAsync(Guid id, CancellationToken cancellationToken)
        {
            return filters.TryGetValue(id, out var filter)
                ? Task.FromResult(filter)
                : throw new NotFoundException($"Filter with ID '{id}' was not found");
        }
    }
}
