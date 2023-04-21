using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public  class EsExceptionBehavior : IClassFixture<TestClientFixture>
    {
        private readonly EsIndexer _indexer;
        private readonly EsIndexTools _indexTools;

        public EsExceptionBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var client = fxt.Client;
            var esClientProvider = new SingleEsClientProvider(client);
            
            _indexer = new EsIndexer(esClientProvider);
            _indexTools = new EsIndexTools(esClientProvider);
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
    }
}
