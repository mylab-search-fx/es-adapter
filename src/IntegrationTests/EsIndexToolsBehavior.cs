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
        private readonly EsIndexTools _indexTools;
        private readonly string _indexName;

        public EsIndexToolsBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            _client = fxt.GetClientProvider(output);
            var esClientProvider = new SingleEsClientProvider(_client);
            _indexTools = new EsIndexTools(esClientProvider);
            _indexName = Guid.NewGuid().ToString("N");
        }

        [Fact]
        public async Task ShouldCreateIndexWithLambdaSettings()
        {
            //Arrange
            GetIndexResponse resp;

            //Act
            await using var deleter = await _indexTools.CreateIndexAsync(_indexName, 
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
            await using var deleter = await _indexTools.CreateIndexAsync(_indexName, settings);
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
            await _indexTools.CreateIndexAsync(_indexName);
            await _indexTools.DeleteIndexAsync(_indexName);
            
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
            await using var deleter = await _indexTools.CreateIndexAsync(_indexName);
            {
                exists = await _indexTools.IsIndexExistsAsync(_indexName);
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange

            //Act
            var exists = await _indexTools.IsIndexExistsAsync("absent");

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldPruneIndex()
        {
            //Arrange
            var testDoc = TestDoc.Generate();

            IIndexRequest<TestDoc> indexReq = new IndexDescriptor<TestDoc>(testDoc, _indexName);

            await using var deleter = await _indexTools.CreateIndexAsync(_indexName);

            var indexResp = await _client.IndexAsync(indexReq);
            EsException.ThrowIfInvalid(indexResp);
            await Task.Delay(1000);

            //Act
            await _indexTools.PruneIndexAsync(_indexName);
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
