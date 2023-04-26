using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class EsIndexerBehavior
    {
        private readonly ElasticClient _client;
        private readonly EsIndexer _indexer;
        private readonly string _indexName;
        private readonly IEsTools _esTools;
        private IAsyncDisposable _indexDeleter;

        public EsIndexerBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _client = fxt.Client;
            var esClientProvider = new SingleEsClientProvider(_client);

            _indexName = Guid.NewGuid().ToString("N");

            _indexer = new EsIndexer(esClientProvider);
            _esTools = new EsTools(esClientProvider);
        }

        private async Task<TestDoc> SearchAsync(string id)
        {
            var res = await _client.SearchAsync(GetSearchFunc());

            EsException.ThrowIfInvalid(res);

            return res.Hits.FirstOrDefault()?.Source;

            Func<SearchDescriptor<TestDoc>, ISearchRequest> GetSearchFunc()
            {
                return sd =>
                    sd.Index(_indexName).Query(qd => qd.Ids(ids => ids.Values(id)));
            }
        }

        public async Task InitializeAsync()
        {
            _indexDeleter = await _esTools.Index(_indexName).CreateAsync();
        }

        public async Task DisposeAsync()
        {
            if (_indexDeleter != null)
                await _indexDeleter.DisposeAsync();
        }
    }
}