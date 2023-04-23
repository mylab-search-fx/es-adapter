using System;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;

namespace IntegrationTests
{
    public class TestPreindexEsFixture : IAsyncLifetime
    {
        private readonly ElasticClient _client;
        private IAsyncDisposable _indexDeleter;

        public string IndexName { get; }

        public TestPreindexEsFixture()
        {
            var settings = new ConnectionSettings(TestTools.ConnectionPool);

            settings.DisableDirectStreaming();

            _client = new ElasticClient(settings);
            IndexName = Guid.NewGuid().ToString("N");
        }

        public async Task InitializeAsync()
        {
            var esClientProvider = new SingleEsClientProvider(_client);

            var indexTools = new EsIndexTools(esClientProvider);

            _indexDeleter = await indexTools.CreateIndexAsync(IndexName, d => d.Map(md => md.AutoMap(typeof(TestDoc))));

            await Task.Delay(1000);

            var indexer = new EsIndexer(esClientProvider);

            await indexer.BulkAsync(IndexName, new EsBulkIndexingRequest<TestDoc>
            {
                CreateList = new[]
                {
                    new TestDoc { Id = "0", Content = "foo-content-0" },
                    new TestDoc { Id = "1", Content = "foo-content-1" },
                    new TestDoc { Id = "2", Content = "foo-content-2" },
                    new TestDoc { Id = "3", Content = "foo-content-3" },
                    new TestDoc { Id = "4", Content = "bar-content-4" },
                    new TestDoc { Id = "5", Content = "bar-content-5" },
                    new TestDoc { Id = "6", Content = "bar-content-6" },
                    new TestDoc { Id = "7", Content = "bar-content-7" },
                }
            });

            await Task.Delay(1000);
        }

        public async Task DisposeAsync()
        {
            if (_indexDeleter != null)
                await _indexDeleter.DisposeAsync();
        }
    }
}