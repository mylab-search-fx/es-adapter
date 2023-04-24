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
        private readonly string _lifecycleExample2Json;

        public EsLifecycleToolsBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            var esClientProvider = new SingleEsClientProvider(client);

            _lcTools = new EsLifecycleTools(esClientProvider);

            _lifecycleExampleJson = File.ReadAllText("Files\\lifecycle-example.json");
            _lifecycleExample2Json = File.ReadAllText("Files\\lifecycle-example-2.json");
        }

        [Fact]
        public async Task ShouldCreateLifecycle()
        {
            //Arrange
            var lcId = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExampleJson);
            var streamExists = await _lcTools.IsLifecyclePolicyExistentAsync(lcId);

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldGetLifecycle()
        {
            //Arrange
            var lcId = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExampleJson);
            var lcPolicy = await _lcTools.TryGetLifecyclePolicyAsync(lcId);

            //Assert
            Assert.NotNull(lcPolicy);
            Assert.Contains(lcPolicy.Policy.Meta, pair => pair.Key == "ver" && (string)pair.Value == "1");
        }

        [Fact]
        public async Task ShouldDeleteLifecycle()
        {
            //Arrange
            var lcId = Guid.NewGuid().ToString("N");
            await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExampleJson);

            //Act
            await _lcTools.DeleteLifecycleAsync(lcId);

            var streamExists = await _lcTools.IsLifecyclePolicyExistentAsync(lcId);

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentLifecycle()
        {
            //Arrange
            var randomId = Guid.NewGuid().ToString("N");

            //Act
            var exists = await _lcTools.IsLifecyclePolicyExistentAsync(randomId);

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldNotGetAbsentLifecycle()
        {
            //Arrange
            var randomId = Guid.NewGuid().ToString("N");

            //Act
            var lifecycle = await _lcTools.TryGetLifecyclePolicyAsync(randomId);

            //Assert
            Assert.Null(lifecycle);
        }

        [Fact]
        public async Task ShouldUpdateLifecycle()
        {
            //Arrange
            var lcId = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExampleJson);
            await _lcTools.PutLifecyclePolicyAsync(lcId, _lifecycleExample2Json);
            var lcPolicy = await _lcTools.TryGetLifecyclePolicyAsync(lcId);

            //Assert
            Assert.NotNull(lcPolicy);
            Assert.Contains(lcPolicy.Policy.Meta, pair => pair.Key == "ver" && (string)pair.Value == "2");
        }
    }
}
