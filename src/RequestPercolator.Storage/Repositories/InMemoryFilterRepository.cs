using Microsoft.Extensions.Options;
using RequestPercolator.Model.Contracts;
using RequestPercolator.Model.Exceptions;
using RequestPercolator.Model.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Storage.Repositories
{
    internal sealed class InMemoryFilterRepository : IFilterRepository
    {
        private readonly InMemoryFilterRepositorySettings repositorySettings;

        public InMemoryFilterRepository(IOptions<InMemoryFilterRepositorySettings> repositorySettingsOptions)
        {
            repositorySettings = repositorySettingsOptions.Value ?? new InMemoryFilterRepositorySettings();
        }

        public Task<string> GetFilterAsync(Guid id, CancellationToken cancellationToken)
        {
            return repositorySettings.Filters.TryGetValue(id.ToString("N"), out var filter)
                ? Task.FromResult(filter)
                : throw new NotFoundException($"Filter with ID '{id}' was not found");
        }
    }
}
