using Microsoft.ReverseProxy.Service.RuntimeModel.Transforms;
using System;

namespace RequestPercolator.API.Proxy.Transforms
{
    internal sealed class RemovePathTransform : RequestParametersTransform
    {
        public override void Apply(RequestParametersTransformContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.Path = string.Empty;
        }
    }
}
