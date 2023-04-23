using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsLifecycleToolsBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsLifecycleTools _lcTools;
        private readonly string _lifecycleExampleJson;

        public EsLifecycleToolsBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            var esClientProvider = new SingleEsClientProvider(client);

            _lcTools = new EsLifecycleTools(esClientProvider);

            _lifecycleExampleJson = File.ReadAllText("Files\\lifecycle-example.json");
        }

        [Fact]
        public async Task ShouldCreateLifecycle()
        {
            //Arrange
            var lcId = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExampleJson, CancellationToken.None);
            var streamExists = await _lcTools.IsLifecyclePolicyExists(lcId);

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldDeleteLifecycle()
        {
            //Arrange
            var lcId = Guid.NewGuid().ToString("N");
            await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExampleJson, CancellationToken.None);

            //Act
            await _lcTools.DeleteLifecycleAsync(lcId);

            var streamExists = await _lcTools.IsLifecyclePolicyExists(lcId);

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentLifecycle()
        {
            //Arrange
            var randomId = Guid.NewGuid().ToString("N");

            //Act
            var exists = await _lcTools.IsLifecyclePolicyExists(randomId);

            //Assert
            Assert.False(exists);
        }
    }
}
