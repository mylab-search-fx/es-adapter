using System;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsIndexToolsBehavior : IClassFixture<TestClientFixture>
    {
        private readonly ElasticClient _client;

        public EsIndexToolsBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            _client = fxt.GetClientProvider(output);
        }

        [Fact]
        public async Task ShouldCreateIndexWithLambdaSettings()
        {
            //Arrange
            var esClientProvider = new SingleEsClientProvider(_client);
            var indexTools = new EsIndexTools(esClientProvider);
            var indexName = Guid.NewGuid().ToString("N");

            GetIndexResponse resp;

            //Act
            await using var deleter = await indexTools.CreateIndexAsync(indexName, 
                d => d.Map(md => md.Properties(p =>
                    p.Text(tpd => tpd.Name("foo")))));
            {
                resp = await _client.Indices.GetAsync(indexName);
            }
            resp.Indices.TryGetValue(indexName, out var indexState);

            //Assert
            Assert.NotNull(indexState);
            Assert.Single(indexState.Mappings.Properties);
            Assert.True(indexState.Mappings.Properties.ContainsKey("foo"));
            Assert.Equal("text", indexState.Mappings.Properties["foo"].Type);
        }

        [Fact]
        public async Task ShouldCreateIndexWithStringSettings()
        {
            //Arrange
            var esClientProvider = new SingleEsClientProvider(_client);
            var indexTools = new EsIndexTools(esClientProvider);
            var indexName = Guid.NewGuid().ToString("N");
            const string settings = "{\"mappings\":{\"properties\":{\"foo\":{\"type\":\"text\"}}}}";

            GetIndexResponse resp;

            //Act
            await using var deleter = await indexTools.CreateIndexAsync(indexName, settings);
            {
                resp = await _client.Indices.GetAsync(indexName);
            }

            resp.Indices.TryGetValue(indexName, out var indexState);

            //Assert
            Assert.NotNull(indexState);
            Assert.Single(indexState.Mappings.Properties);
            Assert.True(indexState.Mappings.Properties.ContainsKey("foo"));
            Assert.Equal("text", indexState.Mappings.Properties["foo"].Type);
        }

        [Fact]
        public async Task ShouldDeleteIndex()
        {
            //Arrange
            var esClientProvider = new SingleEsClientProvider(_client);
            var indexTools = new EsIndexTools(esClientProvider);
            var indexName = Guid.NewGuid().ToString("N");

            //Act
            await indexTools.CreateIndexAsync(indexName);
            await indexTools.DeleteIndexAsync(indexName);
            
            var resp = await _client.Indices.GetAsync(indexName);

            resp.Indices.TryGetValue(indexName, out var indexState);

            //Assert
            Assert.Null(indexState);
        }

        [Fact]
        public async Task ShouldFindExistentIndex()
        {
            //Arrange
            var esClientProvider = new SingleEsClientProvider(_client);
            var indexTools = new EsIndexTools(esClientProvider);
            var indexName = Guid.NewGuid().ToString("N");

            bool exists;

            //Act
            await using var deleter = await indexTools.CreateIndexAsync(indexName);
            {
                exists = await indexTools.IsIndexExistsAsync(indexName);
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange
            var esClientProvider = new SingleEsClientProvider(_client);
            var indexTools = new EsIndexTools(esClientProvider);

            //Act
            var exists = await indexTools.IsIndexExistsAsync("absent");

            //Assert
            Assert.False(exists);
        }
    }
}
