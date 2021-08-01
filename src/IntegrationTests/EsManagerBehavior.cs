using System;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using MyLab.Search.EsAdapter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsManagerBehavior : IClassFixture<ClientFixture>
    {
        private readonly IEsManager _mgr;
        private ClientFixture _clFx;

        public EsManagerBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _mgr = new EsManager(new SingleEsClientProvider(clFx.EsClient), new ElasticsearchOptions());
            _clFx = clFx;
        }

        [Fact]
        public async Task ShouldPing()
        {
            //Arrange
            

            //Act
            var hasPing = await _mgr.PingAsync();

            //Assert
            Assert.True(hasPing);
        }

        [Fact]
        public async Task ShouldNotDetectAbsentIndex()
        {
            //Arrange


            //Act
            var exists = await _mgr.IsIndexExistsAsync("blabla");

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldDetectExistentIndex()
        {
            //Arrange
            var indexName = "test-index-" + Guid.NewGuid().ToString("N");
            bool exists;

            await using (await _mgr.CreateIndexAsync(indexName))
            {
                //Act
                exists = await _mgr.IsIndexExistsAsync(indexName);
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldCreateIndexWithStringSettings()
        {
            //Arrange
            const string settings = "{\"mappings\":{\"properties\":{\"name\":{\"type\":\"text\"}}}}";
            var indexName = "test-index-" + Guid.NewGuid().ToString("N");

            GetIndexResponse indexResp;

            //Act
            await using (await _mgr.CreateIndexAsync(indexName, settings))
            {
                indexResp = await _clFx.EsClient.Indices.GetAsync(indexName);
            }

            var mapping = indexResp.Indices[indexName].Mappings;

            //Assert
            Assert.Equal(1, mapping.Properties.Count);
            Assert.True(mapping.Properties.ContainsKey("name"));
            Assert.Equal("text", mapping.Properties["name"].Type);
        }
    }
}
