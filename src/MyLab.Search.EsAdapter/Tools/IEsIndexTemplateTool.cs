﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special index template
    /// </summary>
    public interface IEsIndexTemplateTool
    {
        /// <summary>
        /// Creates or updates index template
        /// </summary>
        /// <returns>index template deleter</returns>
        Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates index template
        /// </summary>
        /// <returns>index template deleter</returns>
        Task<IAsyncDisposable> PutAsync(Func<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request> putDescriptor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries get index template
        /// </summary>
        /// <returns>index template or null if exists or null</returns>
        public Task<IndexTemplate> TryGetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes index template
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets whether the index template exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }

    class EsIndexTemplateTool : IEsIndexTemplateTool
    {
        private readonly string _templateName;
        private readonly IEsClientProvider _clientProvider;
        private readonly IEsResponseValidator _responseValidator;

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTemplateTool"/>
        /// </summary>
        public EsIndexTemplateTool(string templateName, IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _templateName = templateName;
            _clientProvider = clientProvider;
            _responseValidator = responseValidator;
        }

        public async Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken)
        {
            if (jsonRequest == null) throw new ArgumentNullException(nameof(jsonRequest));
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, "_index_template/" + _templateName, cancellationToken,
                    jsonRequest);

            _responseValidator.Validate(resp, "Unable to put index template");

            return new IndexTemplateDeleter(this);
        }

        public async Task<IAsyncDisposable> PutAsync(Func<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request> putDescriptor, CancellationToken cancellationToken)
        {
            if (putDescriptor == null) throw new ArgumentNullException(nameof(putDescriptor));

            var resp = await _clientProvider.Provide().Indices.PutTemplateV2Async(_templateName, putDescriptor, cancellationToken);

            _responseValidator.Validate(resp, "Unable to put index template");

            return new IndexTemplateDeleter(this);
        }

        public async Task<IndexTemplate> TryGetAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().Indices.GetTemplateV2Async(_templateName, d => d, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return null;

            _responseValidator.Validate(resp, "Unable to get index template");

            return resp.IndexTemplates.FirstOrDefault(t => t.Name == _templateName)?.IndexTemplate;
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().LowLevel.DoRequestAsync<StringResponse>(HttpMethod.DELETE, "_index_template/" + _templateName, cancellationToken);

            _responseValidator.Validate(resp, "Unable to delete index template");
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().LowLevel.DoRequestAsync<StringResponse>(HttpMethod.HEAD, "_index_template/" + _templateName, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return false;

            _responseValidator.Validate(resp, "Unable to detect index template");

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