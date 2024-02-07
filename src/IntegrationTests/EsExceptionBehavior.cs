using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public  class EsExceptionBehavior : IClassFixture<TestClientFixture>, IAsyncLifetime
    {
        private readonly EsIndexer _indexer;
        private readonly ElasticClient _client;

        public EsExceptionBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _client = fxt.Client;
            var esClientProvider = new SingleEsClientProvider(_client);
            
            _indexer = new EsIndexer(esClientProvider, TestTools.ResponseValidator);
        }

        [Fact]
        public async Task ShouldDetectIndexNotFoundWhenIndexing()
        {
            //Arrange
            EsException exception = null;

            var doc = TestDoc.Generate();

            //Act
            
            try
            {
                await _indexer.CreateAsync("foo", doc);
            }
            catch (EsException e)
            {
                exception = e;
            }

            //Assert
            Assert.NotNull(exception);
            Assert.True(exception.Response.HasIndexNotFound);
        }

        [Fact]
        public async Task ShouldDetectIndexNotFoundWhenBulk()
        {
            //Arrange
            EsException exception = null;

            var doc = TestDoc.Generate();

            //Act

            try
            {
                await _indexer.BulkAsync<TestDoc>("foo", d => d.AddOperation(new BulkCreateOperation<TestDoc>(doc)));
            }
            catch (EsException e)
            {
                exception = e;
            }

            //Assert
            Assert.NotNull(exception);
            Assert.True(exception.Response.HasIndexNotFound);
        }

        public Task InitializeAsync()
        {
            return _client.Indices.DeleteAsync("foo");
        }

        public Task DisposeAsync()
        {
            return _client.Indices.DeleteAsync("foo");
        }
    }
}
