using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsIndexToolBehavior : IClassFixture<TestClientFixture>
    {
        private readonly ElasticClient _client;
        private readonly IEsIndexTool _indexTool;
        private readonly string _indexName;
        private readonly SingleEsClientProvider _esClientProvider;

        public EsIndexToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _client = fxt.Client;
            _esClientProvider = new SingleEsClientProvider(_client);

            _indexName = Guid.NewGuid().ToString("N");
            _indexTool = new EsIndexTool(_indexName, _esClientProvider);
        }

        [Fact]
        public async Task ShouldCreateIndexWithLambdaSettings()
        {
            //Arrange
            GetIndexResponse resp;

            //Act
            await using var deleter = await _indexTool.CreateAsync(
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
        public async Task ShouldGetIndexInfo()
        {
            //Arrange
            IndexState indexInfo;

            //Act
            await using var deleter = await _indexTool.CreateAsync(
                d => d.Map(md => md.Properties(p =>
                    p.Text(tpd => tpd.Name("foo")))));
            {
                indexInfo = await _indexTool.TryGetAsync();
            }

            //Assert
            Assert.NotNull(indexInfo);
            Assert.True(indexInfo.Mappings.Properties.ContainsKey("foo"));
            Assert.Equal("text", indexInfo.Mappings.Properties["foo"].Type);
        }

        [Fact]
        public async Task ShouldCreateIndexWithStringSettings()
        {
            //Arrange
            const string settings = "{\"mappings\":{\"properties\":{\"foo\":{\"type\":\"text\"}}}}";

            GetIndexResponse resp;

            //Act
            await using var deleter = await _indexTool.CreateAsync(settings);
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
            await _indexTool.CreateAsync();
            await _indexTool.DeleteAsync();
            
            var resp = await _client.Indices.GetAsync(_indexName);

            resp.Indices.TryGetValue(_indexName, out var indexState);

            //Assert
            Assert.Null(indexState);
        }

        [Fact]
        public async Task ShouldNotUpdateExistentIndex()
        {
            //Arrange
            await _indexTool.CreateAsync();

            //Act & Assert
            await Assert.ThrowsAsync<EsException>(() => _indexTool.CreateAsync());
        }

        [Fact]
        public async Task ShouldFindExistentIndex()
        {
            //Arrange

            bool exists;

            //Act
            await using var deleter = await _indexTool.CreateAsync();
            {
                exists = await _indexTool.ExistsAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange

            //Act
            var exists = await new EsIndexTool("absent", _esClientProvider).ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldNotGetInfoOfAbsentIndex()
        {
            //Arrange

            //Act
            var indexInfo = await new EsIndexTool("absent", _esClientProvider).TryGetAsync();

            //Assert
            Assert.Null(indexInfo);
        }

        [Fact]
        public async Task ShouldPruneIndex()
        {
            //Arrange
            var testDoc = TestDoc.Generate();

            IIndexRequest<TestDoc> indexReq = new IndexDescriptor<TestDoc>(testDoc, _indexName);

            await using var deleter = await _indexTool.CreateAsync();

            var indexResp = await _client.IndexAsync(indexReq);
            EsException.ThrowIfInvalid(indexResp);
            await Task.Delay(1000);

            //Act
            await _indexTool.PruneAsync(CancellationToken.None);
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

        [Fact]
        public async Task ShouldPutMapping()
        {
            //Arrange
            const string settings = "{\"mappings\":{\"properties\":{\"foo\":{\"type\":\"text\"}}}}";
            const string newMapping = "{\"properties\":{\"bar\":{\"type\":\"text\"}}}";

            IndexState idxState;

            //Act
            await using var deleter = await _indexTool.CreateAsync(settings);
            {
                await _indexTool.PutMapping(newMapping);
                idxState = await _indexTool.TryGetAsync();
            }
            
            //Assert
            Assert.NotNull(idxState);
            Assert.Single(idxState.Mappings.Properties);
            Assert.True(idxState.Mappings.Properties.ContainsKey("bar"));
        }
    }
}
