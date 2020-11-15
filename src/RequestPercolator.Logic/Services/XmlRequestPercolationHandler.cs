using Microsoft.AspNetCore.Http;
using RequestPercolator.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace RequestPercolator.Logic.Services
{
    internal sealed class XmlRequestPercolationHandler : MimeTypeRequestPercolationHandler
    {
        protected override IReadOnlyCollection<string> SupportedMimeTypes { get; } = new[]
        {
            Application.Xml
        };

        public override async Task<PercolationResult> HandleAsync(HttpRequest request, string filter, CancellationToken cancellationToken)
        {
            var element = await XElement.LoadAsync(request.Body, LoadOptions.PreserveWhitespace, cancellationToken);
            var success = element.XPathEvaluate(filter) switch
            {
                bool boolValue => boolValue,
                string stringValue => !string.IsNullOrWhiteSpace(stringValue),
                double doubleValue => doubleValue > 0,
                { } => true
            };
            return success
                ? PercolationResult.Success
                : PercolationResult.Failed;
        }
    }
}
