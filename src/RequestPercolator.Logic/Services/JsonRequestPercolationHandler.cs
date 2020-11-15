using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RequestPercolator.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RequestPercolator.Logic.Services
{
    internal sealed class JsonRequestPercolationHandler : MimeTypeRequestPercolationHandler
    {
        private readonly JsonLoadSettings jsonLoadSettings;

        public JsonRequestPercolationHandler(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(JsonRequestPercolationHandler));

            jsonLoadSettings = section.Get<JsonLoadSettings>()
                ?? new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore,
                    LineInfoHandling = LineInfoHandling.Ignore,
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Ignore
                };
        }

        protected override IReadOnlyCollection<string> SupportedMimeTypes { get; } = new[]
        {
            Application.Json,
            "application/problem+json"
        };

        public override async Task<PercolationResult> HandleAsync(HttpRequest request, string filter, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(request.Body);
            using var jsonReader = new JsonTextReader(reader);
            while (jsonReader.TokenType != JsonToken.StartObject && jsonReader.TokenType != JsonToken.StartArray)
            {
                if (!await jsonReader.ReadAsync(cancellationToken))
                {
                    return PercolationResult.Failed;
                }
            }

            var array = jsonReader.TokenType switch
            {
                JsonToken.StartObject => new JArray(await JObject.LoadAsync(jsonReader, jsonLoadSettings, cancellationToken)),
                JsonToken.StartArray => await JArray.LoadAsync(jsonReader, jsonLoadSettings, cancellationToken),
                _ => throw new FormatException("Object or array start was not found"),
            };

            return array.SelectToken(filter) is null
                ? PercolationResult.Failed
                : PercolationResult.Success;
        }

        public override bool CanHandleFilter(string filter)
        {
            try
            {
                new JObject().SelectToken(filter);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
