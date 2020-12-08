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

        public EsIndexerBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
            _indexer = new EsIndexer<TestEntity>(new SingleEsClientProvider(_clFx.EsClient), null);
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
    }
}