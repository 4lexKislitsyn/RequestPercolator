using System.Collections.Generic;

namespace RequestPercolator.Model.Settings
{
    public sealed class InMemoryFilterRepositorySettings
    {
        public IDictionary<string, string> Filters { get; } = new Dictionary<string, string>();
    }
}
