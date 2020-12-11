using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using MyLab.Elastic;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsIndexerBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        private readonly ITestOutputHelper _output;
        private readonly IEsIndexer<TestEntity> _indexer;
        private readonly EsSearcher<TestEntity> _searcher;

        public EsIndexerBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
            _clFx.Output = output;
            _indexer = new EsIndexer<TestEntity>(new SingleEsClientProvider(_clFx.EsClient), null, (ElasticsearchOptions)null);
            _searcher = new EsSearcher<TestEntity>(new SingleEsClientProvider(_clFx.EsClient), null, (ElasticsearchOptions)null);
        }

        [Fact]
        public async Task ShouldIndexDocument()
        {
            //Arrange
            var document = new TestEntity
            {
                Id = 10,
                Value = "foo"
            };

            //Act & Assert
            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                await _indexer.IndexAsync(indNm, document);
            });
        }

        [Fact]
        public async Task ShouldNotIndexDocumentWhenIndexDoesNotExists()
        {
            //Arrange
            var document = new TestEntity
            {
                Id = 10,
                Value = "foo"
            };

            //Act & Assert
            await Assert.ThrowsAsync<EsIndexException>(() => _indexer.IndexAsync(" absent-index", document));
        }

        [Fact]
        public async Task ShouldUpdateDocument()
        {
            //Arrange
            var document = new TestEntity
            {
                Id = 10,
                Value = "foo"
            };
            EsFound<TestEntity> found = null;

            //Act
            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                await _indexer.IndexAsync(indNm, document);

                await _indexer.UpdateAsync(indNm, document.Id, () => new TestEntity
                {
                    Value = "bar"
                });

                await Task.Delay(1000);

                found = await _searcher.SearchAsync(indNm, new SearchParams<TestEntity>(d => d.Ids(ids => ids.Values(document.Id))));
            });

            //Assert
            Assert.NotNull(found);
            Assert.Single(found);
            Assert.Equal("bar", found.First().Value);
        }
    }
}