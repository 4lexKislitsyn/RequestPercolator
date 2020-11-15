using Microsoft.AspNetCore.Http;
using RequestPercolator.Logic.Contracts;
using RequestPercolator.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.Logic.Services
{
    internal abstract class MimeTypeRequestPercolationHandler : IRequestPercolationHandler
    {
        protected abstract IReadOnlyCollection<string> SupportedMimeTypes { get; }

        public virtual bool CanHandleRequest(HttpRequest request)
            => !string.IsNullOrWhiteSpace(request?.ContentType) && SupportedMimeTypes.Contains(request.ContentType);

        public virtual bool CanHandleFilter(string filter) => true;

        public abstract Task<PercolationResult> HandleAsync(HttpRequest request, string filter, CancellationToken cancellationToken);
    }
}
