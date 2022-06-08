using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter
{
    public class EsIndexTools : IEsIndexTools
    {
        private readonly IEsClientProvider _clientProvider;

        public EsIndexTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<IIndexDeleter> CreateIndexAsync(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.CreateAsync(indexName, createDescriptor, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index");

            return new IndexDeleter(indexName, this);
        }

        public async Task<IIndexDeleter> CreateIndexAsync(string indexName, string jsonSettings, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel.Indices.CreateAsync<CreateIndexResponse>(indexName, jsonSettings, null, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index");

            return new IndexDeleter(indexName, this);
        }

        public async Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.DeleteAsync(indexName, ct: cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete the index");
        }

        public async Task<bool> IsIndexExistsAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.GetAsync(indexName, ct: cancellationToken);

            if (!resp.IsValid && resp.ServerError.Status == 404)
                return false;

            EsException.ThrowIfInvalid(resp);

            return resp.Indices.ContainsKey(indexName);
        }

        class IndexDeleter : IIndexDeleter
        {
            private readonly string _indexName;
            private readonly IEsIndexTools _idxTools;

            public IndexDeleter(string indexName, IEsIndexTools idxTools)
            {
                _indexName = indexName;
                _idxTools = idxTools;
            }

            public async ValueTask DisposeAsync()
            {
                await _idxTools.DeleteIndexAsync(_indexName);
            }
        }
    }
}