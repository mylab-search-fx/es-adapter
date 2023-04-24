using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Default implementation for <see cref="IEsIndexTools"/>
    /// </summary>
    public class EsIndexTools : IEsIndexTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsIndexTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> CreateIndexAsync(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.CreateAsync(indexName, createDescriptor, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index");

            return new IndexDeleter(indexName, this);
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> CreateIndexAsync(string indexName, string jsonSettings, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel.Indices.CreateAsync<CreateIndexResponse>(indexName, jsonSettings, null, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index");

            return new IndexDeleter(indexName, this);
        }

        /// <inheritdoc />
        public async Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.DeleteAsync(indexName, ct: cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete the index");
        }

        /// <inheritdoc />
        public async Task<bool> IsIndexExistentAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.GetAsync(indexName, ct: cancellationToken);

            if (!resp.IsValid && resp.ServerError.Status == 404)
                return false;

            EsException.ThrowIfInvalid(resp);

            return resp.Indices.ContainsKey(indexName);
        }

        /// <inheritdoc />
        public async Task PruneIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var req = new DeleteByQueryRequest(indexName)
            {
                Query = new QueryContainer(new MatchAllQuery())
            };
            await _clientProvider.Provide().DeleteByQueryAsync(req, cancellationToken);
        }

        class IndexDeleter : IAsyncDisposable
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