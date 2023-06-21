using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class IndexAliasToolBehavior : IClassFixture<TestClientFixture>, IAsyncLifetime
    {
        private readonly IEsIndexTool _indexTool;

        public IndexAliasToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var client = fxt.Client;
            var esClientProvider = new SingleEsClientProvider(client);

            var indexName = Guid.NewGuid().ToString("N");
            _indexTool = new EsIndexTool(indexName, esClientProvider);
        }

        public async Task InitializeAsync()
        {
            await _indexTool.CreateAsync(d => d.Aliases(s => s.Alias("bar")));
        }

        public async Task DisposeAsync()
        {
            bool indexExists = await _indexTool.ExistsAsync();
            if (indexExists)
            {
                await _indexTool.DeleteAsync();
            }
        }

        [Fact]
        public async Task ShouldDetectExistentAlias()
        {
            //Arrange
            var tool = _indexTool.Alias("bar");

            //Act
            var exists = await tool.ExistsAsync();

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotDetectAbsentAlias()
        {
            //Arrange
            var tool = _indexTool.Alias("absent");

            //Act
            var exists = await tool.ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldGetAlias()
        {
            //Arrange
            await using var aliasDeleter = await _indexTool.Alias("foo").PutAsync();

            var tool = _indexTool.Alias("foo");

            //Act
            var aliasDef = await tool.GetAsync();

            //Assert
            Assert.NotNull(aliasDef);
        }

        [Fact]
        public async Task ShouldDeleteAlias()
        {
            //Arrange
            var aliasDeleter = await _indexTool.Alias("foo").PutAsync();

            bool aliasExists = false;
            try
            {
                var tool = _indexTool.Alias("foo");
                await tool.DeleteAsync();

                //Act
                aliasExists = await tool.ExistsAsync();

                if (aliasExists)
                    await aliasDeleter.DisposeAsync();
            }
            catch (Exception)
            {
                await aliasDeleter.DisposeAsync();
            }

            //Assert
            Assert.False(aliasExists);
        }

        [Fact]
        public async Task ShouldCreateAlias()
        {
            //Arrange
            await using var aliasDeleter = await _indexTool.Alias("foo").PutAsync();

            var tool = _indexTool.Alias("foo");

            //Act
            var exists = await tool.ExistsAsync();

            //Assert
            Assert.True(exists);
        }
    }
}
