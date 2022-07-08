using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly ElasticClient _client;
        private readonly EsIndexer _indexer;
        private readonly EsIndexTools _indexTools;

        public EsExceptionBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            _client = fxt.GetClientProvider(output);
            var esClientProvider = new SingleEsClientProvider(_client);
            
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
            Assert.True(exception.HasIndexNotFound());
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
            Assert.True(exception.HasIndexNotFound());
        }
    }
}
