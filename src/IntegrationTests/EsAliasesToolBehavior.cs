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
    public class EsAliasesToolBehavior : IClassFixture<TestClientFixture>, IAsyncLifetime
    {
        private readonly IEsAliasesTool _aliasesTool;
        private readonly EsIndexTool _indexTool;

        public EsAliasesToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var esClientProvider = new SingleEsClientProvider(fxt.Client);
            _aliasesTool = new EsAliasesTool(esClientProvider, TestTools.ResponseValidator);

            var indexName = Guid.NewGuid().ToString("N");
            _indexTool = new EsIndexTool(indexName, esClientProvider, TestTools.ResponseValidator);
        }

        [Fact]
        public async Task ShouldEnumerateAliases()
        {
            //Arrange
            var aliasName = Guid.NewGuid().ToString("N");

            await _indexTool.Alias(aliasName).PutAsync();

            //Act
            var aliases = await _aliasesTool.GetAliasesAsync();
            var aliasesArray = aliases.ToArray();

            //Assert
            Assert.Contains(aliasesArray, a => a.AliasName == aliasName);
        }

        [Fact]
        public async Task ShouldEnumerateEmptyWhenNotFound()
        {
            //Arrange
            var absentAliasName = Guid.NewGuid().ToString("N");

            //Act
            var aliases = await _aliasesTool.GetAliasesAsync(a => a.Name(absentAliasName));
            var aliasesArray = aliases.ToArray();

            //Assert
            Assert.Empty(aliasesArray);
        }

        [Fact]
        public async Task ShouldFilterAliases()
        {
            //Arrange
            var aliasName = Guid.NewGuid().ToString("N");

            await _indexTool.Alias(aliasName).PutAsync();

            //Act
            var aliases = await _aliasesTool.GetAliasesAsync(s => s.Name(aliasName));
            var aliasesArray = aliases.ToArray();

            //Assert
            Assert.Single(aliasesArray);
            Assert.Contains(aliasesArray, a => a.AliasName == aliasName);
        }

        public Task InitializeAsync()
        {
            return _indexTool.CreateAsync();
        }

        public async Task DisposeAsync()
        {
            bool exists = await _indexTool.ExistsAsync();
            if (exists)
            {
                await _indexTool.DeleteAsync();
            }
        }
    }
}
