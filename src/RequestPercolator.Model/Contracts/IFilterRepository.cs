using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Model.Contracts
{
    public interface IFilterRepository
    {
        Task<string> GetFilterAsync(Guid id, CancellationToken cancellationToken);
    }
}
