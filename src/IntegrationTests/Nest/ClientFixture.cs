using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace IntegrationTests.Nest
{
    public class ClientFixture : IDisposable
    {
        private readonly IConnectionPool _connectionPool;

        public ElasticClient EsClient { get; }

        public ClientFixture()
        {
            var testEsAddr = Environment.GetEnvironmentVariable("TEST_ES_ADDR");

            if (string.IsNullOrEmpty(testEsAddr))
                throw new InvalidOperationException("TEST_ES_ADDR must be set");

            var uri = new Uri((!testEsAddr.StartsWith("http") ? "http://" : "") + testEsAddr);
            _connectionPool = new SingleNodeConnectionPool(uri);
            var settings = new ConnectionSettings(_connectionPool);
            EsClient = new ElasticClient(settings);
        }

        public void Dispose()
        {
            _connectionPool?.Dispose();
        }

        public async Task UseTmpIndex(Func<string, Task> action, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null)
        {
            string indexName = await CreateIndex(selector);
            try
            {
                await action(indexName);
            }
            finally
            {
                await DeleteIndex(indexName);
            }
        }

        public async Task<TRes> UseTmpIndex<TRes>(Func<string, Task<TRes>> action, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null)
        {
            string indexName = await CreateIndex(selector);
            try
            {
                return await action(indexName);
            }
            finally
            {
                await DeleteIndex(indexName);
            }
        }

        async Task<string> CreateIndex(Func<CreateIndexDescriptor, ICreateIndexRequest> selector)
        {
            var indexName = "test-" + Guid.NewGuid().ToString("N");

            var res = await EsClient.Indices.CreateAsync(
                indexName, selector);

            if (!res.ShardsAcknowledged)
                throw new InvalidOperationException("Could not create index");

            return indexName;
        }

        async Task DeleteIndex(string name)
        {
            await EsClient.Indices.DeleteAsync(name);
        }
    }
}
