using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    class EsIndexTemplateTool : IEsIndexTemplateTool
    {
        private readonly string _templateName;
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTemplateTool"/>
        /// </summary>
        public EsIndexTemplateTool(string templateName, IEsClientProvider clientProvider)
        {
            _templateName = templateName;
            _clientProvider = clientProvider;
        }

        public async Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken)
        {
            if (jsonRequest == null) throw new ArgumentNullException(nameof(jsonRequest));
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, "_index_template/" + _templateName, cancellationToken,
                    jsonRequest);

            EsException.ThrowIfInvalid(resp, "Unable to put index template");

            return new IndexTemplateDeleter(this);
        }

        public async Task<IAsyncDisposable> PutAsync(IPutIndexTemplateV2Request request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            await _clientProvider.Provide().Indices.PutTemplateV2Async(request, cancellationToken);

            return new IndexTemplateDeleter(this);
        }

        public async Task<IndexTemplate> TryGetAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().Indices.GetTemplateV2Async(_templateName, d => d, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return null;

            EsException.ThrowIfInvalid(resp, "Unable to get index template");

            return resp.IndexTemplates.FirstOrDefault(t => t.Name == _templateName)?.IndexTemplate;
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().LowLevel.DoRequestAsync<StringResponse>(HttpMethod.DELETE, "_index_template/" + _templateName, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete index template");
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().LowLevel.DoRequestAsync<StringResponse>(HttpMethod.HEAD, "_index_template/" + _templateName, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return false;

            EsException.ThrowIfInvalid(resp, "Unable to detect index template");

            return true;
        }

        class IndexTemplateDeleter : IAsyncDisposable
        {
            private readonly IEsIndexTemplateTool _tool;

            public IndexTemplateDeleter(IEsIndexTemplateTool tool)
            {
                _tool = tool;
            }
            public async ValueTask DisposeAsync()
            {
                await _tool.DeleteAsync(CancellationToken.None);
            }
        }
    }
}