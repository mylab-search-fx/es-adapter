using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    class EsIndexTool : IEsIndexTool
    {
        private readonly string _indexName;
        private readonly IEsClientProvider _clientProvider;

        public EsIndexTool(string indexName, IEsClientProvider clientProvider)
        {
            _indexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        }

        public async Task<IAsyncDisposable> CreateAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.CreateAsync(_indexName, createDescriptor, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index");

            return new IndexDeleter(this);
        }

        public async Task<IAsyncDisposable> CreateAsync(string jsonSettings, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel.Indices.CreateAsync<CreateIndexResponse>(_indexName, jsonSettings, null, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index");

            return new IndexDeleter(this);
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.DeleteAsync(_indexName, ct: cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete the index");
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.GetAsync(_indexName, ct: cancellationToken);

            if (!resp.IsValid && resp.ServerError.Status == 404)
                return false;

            EsException.ThrowIfInvalid(resp);

            return resp.Indices.ContainsKey(_indexName);
        }

        public async Task PruneAsync(CancellationToken cancellationToken = default)
        {
            var req = new DeleteByQueryRequest(_indexName)
            {
                Query = new QueryContainer(new MatchAllQuery())
            };
            await _clientProvider.Provide().DeleteByQueryAsync(req, cancellationToken);
        }

        public async Task<IndexState> TryGet(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.GetAsync(_indexName, s => s, cancellationToken);

            if (resp.Indices != null && resp.Indices.TryGetValue(_indexName, out var foundIndex))
            {
                return foundIndex;
            }

            if (resp.ApiCall.HttpStatusCode == 404)
                return null;

            EsException.ThrowIfInvalid(resp);

            return null;
        }

        class IndexDeleter : IAsyncDisposable
        {
            private readonly IEsIndexTool _idxTool;

            public IndexDeleter(IEsIndexTool idxTool)
            {
                _idxTool = idxTool;
            }

            public async ValueTask DisposeAsync()
            {
                await _idxTool.DeleteAsync();
            }
        }
    }
}