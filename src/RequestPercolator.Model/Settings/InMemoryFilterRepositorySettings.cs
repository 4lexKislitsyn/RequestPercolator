using System;
using System.Collections.Generic;

namespace RequestPercolator.Model.Settings
{
    public sealed class InMemoryFilterRepositorySettings
    {
        public IDictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
