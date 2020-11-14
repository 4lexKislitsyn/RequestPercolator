using Microsoft.ReverseProxy.Service.RuntimeModel.Transforms;
using System;
using System.Linq;

namespace RequestPercolator.API.Proxy.Transforms
{
    internal sealed class RemoveQueryParametersByPrefixTransform : RequestParametersTransform
    {
        private readonly string prefix;

        public RemoveQueryParametersByPrefixTransform(string prefix)
        {
            this.prefix = prefix;
        }

        public override void Apply(RequestParametersTransformContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var parametersToRemove = context.Query.Collection.Keys
                .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            parametersToRemove.ForEach(x => context.Query.Collection.Remove(x));
        }
    }
}
