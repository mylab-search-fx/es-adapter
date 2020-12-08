using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Elastic
{
    class EsManager : IEsManager
    {
        private readonly ElasticsearchOptions _options;
        private readonly ElasticClient _client;

        public EsManager(IEsClientProvider clientProvider, IOptions<ElasticsearchOptions> options)
            :this(clientProvider, options.Value)
        {
        }

        public EsManager(IEsClientProvider clientProvider, ElasticsearchOptions options)
        {
            _options = options;
            _client = clientProvider.Provide();
        }

        public async Task<bool> PingAsync()
        {
            var resp = await _client.PingAsync();

            return resp.IsValid;
        }

        public async Task<IAsyncDisposable> CreateIndexAsync(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null)
        {
            var res = await _client.Indices.CreateAsync(
                indexName, selector);

            if(!res.IsValid)
                throw new ResponseException("Can't create index", res);

            return new IndexDeleter(indexName, this);
        }

        public Task<IAsyncDisposable> CreateDefaultIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null)
        {
            return CreateIndexAsync(_options.DefaultIndex, selector);
        }

        public async Task DeleteIndexAsync(string indexName)
        {
            await _client.Indices.DeleteAsync(indexName);
        }

        class IndexDeleter : IAsyncDisposable
        {
            private readonly string _indexName;
            private readonly IEsManager _esMgr;

            public IndexDeleter(string indexName, IEsManager esMgr)
            {
                _indexName = indexName;
                _esMgr = esMgr;
            }

            public async ValueTask DisposeAsync()
            {
                await _esMgr.DeleteIndexAsync(_indexName);
            }
        }
    }
}