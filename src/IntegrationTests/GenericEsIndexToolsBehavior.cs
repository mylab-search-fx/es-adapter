using System;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class GenericEsIndexToolsBehavior : IClassFixture<TestClientFixture>
    {
        [Fact]
        public async Task ShouldCreateIndexWithLambdaSettings()
        {
            //Arrange
            GetIndexResponse resp;

            //Act
            await using var deleter = await _indexTools.CreateIndexAsync( 
                d => d.Map(md => md.Properties(p =>
                    p.Text(tpd => tpd.Name("foo")))));
            {
                resp = await _client.Indices.GetAsync(_indexName);
            }
            resp.Indices.TryGetValue(_indexName, out var indexState);

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
            const string settings = "{\"mappings\":{\"properties\":{\"foo\":{\"type\":\"text\"}}}}";

            GetIndexResponse resp;

            //Act
            await using var deleter = await _indexTools.CreateIndexAsync(settings);
            {
                resp = await _client.Indices.GetAsync(_indexName);
            }

            resp.Indices.TryGetValue(_indexName, out var indexState);

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

            //Act
            await _indexTools.CreateIndexAsync();
            await _indexTools.DeleteIndexAsync();
            
            var resp = await _client.Indices.GetAsync(_indexName);

            resp.Indices.TryGetValue(_indexName, out var indexState);

            //Assert
            Assert.Null(indexState);
        }

        [Fact]
        public async Task ShouldFindExistentIndex()
        {
            //Arrange

            bool exists;

            //Act
            await using var deleter = await _indexTools.CreateIndexAsync();
            {
                exists = await _indexTools.IsIndexExistsAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange

            //Act
            var exists = await _indexTools.IsIndexExistsAsync();

            //Assert
            Assert.False(exists);
        }
    }
}
