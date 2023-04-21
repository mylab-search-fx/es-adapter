using System;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class SpecialEsIndexToolsBehavior : IClassFixture<TestClientFixture>
    {
        [Fact]
        public async Task ShouldCreateIndexWithLambdaSettings()
        {
            //Arrange
            GetIndexResponse resp;

            //Act
            await using var deleter = await _specialIndexTools.CreateIndexAsync( 
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
            await using var deleter = await _specialIndexTools.CreateIndexAsync(settings);
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
            await _specialIndexTools.CreateIndexAsync();
            await _specialIndexTools.DeleteIndexAsync();
            
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
            await using var deleter = await _specialIndexTools.CreateIndexAsync();
            {
                exists = await _specialIndexTools.IsIndexExistsAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange

            //Act
            var exists = await _specialIndexTools.IsIndexExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldPruneIndex()
        {
            //Arrange
            var testDoc = TestDoc.Generate();

            IIndexRequest<TestDoc> indexReq = new IndexDescriptor<TestDoc>(testDoc, _indexName);

            await using var deleter = await _specialIndexTools.CreateIndexAsync();

            var indexResp = await _client.IndexAsync(indexReq);
            EsException.ThrowIfInvalid(indexResp);
            await Task.Delay(1000);

            //Act
            await _specialIndexTools.PruneIndexAsync();
            await Task.Delay(1000);

            var searchResp = await _client.SearchAsync<TestDoc>(CreateSearch);

            //Assert
            Assert.Equal(0, searchResp.Total);

            ISearchRequest CreateSearch(SearchDescriptor<TestDoc> d)
            {
                return d
                    .Index(Indices.Index(_indexName))
                    .MatchAll();
            }
        }
    }
}
