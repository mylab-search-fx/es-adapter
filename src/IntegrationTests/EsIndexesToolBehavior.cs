using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsIndexesToolBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsIndexesTool _indexesTool;
        private readonly EsIndexTool _testIndex;
        private readonly string _testIndexName;

        public EsIndexesToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var esClientProvider = new SingleEsClientProvider(fxt.Client);
            _indexesTool = new EsIndexesTool(esClientProvider, TestTools.ResponseValidator);

            _testIndexName = Guid.NewGuid().ToString("N");
            _testIndex = new EsIndexTool(_testIndexName, esClientProvider, TestTools.ResponseValidator);
        }

        [Fact]
        public async Task ShouldCreateIndexWithRequest()
        {
            //Arrange
            ICreateIndexRequest newIndexReq = new CreateIndexRequest(_testIndexName);

            //Act
            await _indexesTool.CreateAsync(newIndexReq);

            bool exists = await _testIndex.ExistsAsync();

            if (exists)
            {
                await _testIndex.DeleteAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldCreateIndexWithSettings()
        {
            //Arrange
            var settings = "{}";

            //Act
            await _indexesTool.CreateAsync(_testIndexName, settings);

            bool exists = await _testIndex.ExistsAsync();

            if (exists)
            {
                await _testIndex.DeleteAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldEnumerateIndexes()
        {
            //Arrange
            await _testIndex.CreateAsync();

            //Act
            var indexes = await _indexesTool.GetAsync();
            var indexesArray = indexes.ToArray();

            //Assert
            Assert.Contains(indexesArray, a => a.Name == _testIndexName);
        }

        [Fact]
        public async Task ShouldFilterIndexes()
        {
            //Arrange
            await _testIndex.CreateAsync();

            //Act
            var indexes = await _indexesTool.GetAsync(d => d.Index(_testIndexName));
            var indexesArray = indexes.ToArray();

            //Assert
            Assert.Contains(indexesArray, a => a.Name == _testIndexName);
        }

        [Fact]
        public async Task ShouldDeleteIndexes()
        {
            //Arrange
            await _testIndex.CreateAsync();

            //Act
            await _indexesTool.DeleteAsync(d => d.Index(_testIndexName));

            var exists = await _testIndex.ExistsAsync();

            //Assert
            Assert.False(exists);
        }
    }
}
