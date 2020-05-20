using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace IntegrationTests.Stuff
{
    public class ClientFixture : IDisposable
    {
        private readonly IConnectionPool _connectionPool;

        public ElasticClient EsClient { get; }

        public ClientFixture()
        {
            _connectionPool = TestConnection.Create();
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

        public Task UseTmpIndexWithMap(Func<string, Task> action)
        {
            return UseTmpIndex(action, cd =>
                cd.Map<TestEntity>(md => md.AutoMap()));
        }

        public Task<TRes> UseTmpIndexWithMap<TRes>(Func<string, Task<TRes>> action)
        {
            return UseTmpIndex(action, cd =>
                cd.Map<TestEntity>(md => md.AutoMap()));
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
