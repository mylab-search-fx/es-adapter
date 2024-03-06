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
    public class EsLifecyclePolicyToolBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsLifecyclePolicyTool _lcTool;
        private readonly string _lifecycleExampleJson;
        private readonly string _lifecycleExample2Json;
        private readonly SingleEsClientProvider _esClientProvider;
        private readonly string _lcId;

        public EsLifecyclePolicyToolBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            _esClientProvider = new SingleEsClientProvider(client);
            _lcId = Guid.NewGuid().ToString("N");

            _lcTool = new EsLifecyclePolicyTool(_lcId, _esClientProvider, TestTools.ResponseValidator);

            _lifecycleExampleJson = File.ReadAllText("Files\\lifecycle-example.json");
            _lifecycleExample2Json = File.ReadAllText("Files\\lifecycle-example-2.json");
        }

        [Fact]
        public async Task ShouldCreateLifecycle()
        {
            //Arrange

            //Act
            await using var deleter = await _lcTool.PutAsync(_lifecycleExampleJson);
            var streamExists = await _lcTool.ExistsAsync();

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldGetLifecycle()
        {
            //Arrange

            //Act
            await using var deleter = await _lcTool.PutAsync(_lifecycleExampleJson);
            var lcPolicy = await _lcTool.TryGetAsync();

            //Assert
            Assert.NotNull(lcPolicy);
            Assert.Contains(lcPolicy.Policy.Meta, pair => pair.Key == "ver" && (string)pair.Value == "1");
        }

        [Fact]
        public async Task ShouldDeleteLifecycle()
        {
            //Arrange
            await _lcTool.PutAsync(_lifecycleExampleJson);

            //Act
            await _lcTool.DeleteAsync();

            var streamExists = await _lcTool.ExistsAsync();

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentLifecycle()
        {
            //Arrange
            var randomId = Guid.NewGuid().ToString("N");

            //Act
            var exists = await new EsLifecyclePolicyTool(randomId, _esClientProvider, TestTools.ResponseValidator).ExistsAsync(CancellationToken.None);

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldNotGetAbsentLifecycle()
        {
            //Arrange
            var randomId = Guid.NewGuid().ToString("N");

            //Act
            var lifecycle = await new EsLifecyclePolicyTool(randomId, _esClientProvider, TestTools.ResponseValidator).TryGetAsync(CancellationToken.None);

            //Assert
            Assert.Null(lifecycle);
        }

        [Fact]
        public async Task ShouldUpdateLifecycle()
        {
            //Arrange

            //Act
            await using var deleter = await _lcTool.PutAsync(_lifecycleExampleJson);
            await _lcTool.PutAsync(_lifecycleExample2Json);
            var lcPolicy = await _lcTool.TryGetAsync();

            //Assert
            Assert.NotNull(lcPolicy);
            Assert.Contains(lcPolicy.Policy.Meta, pair => pair.Key == "ver" && (string)pair.Value == "2");
        }

        [Fact]
        public async Task ShouldFailWhenEsErrorResponse()
        {
            //Arrange 
            var invalidPolicyRequest = new PutLifecycleRequest(_lcId);

            //Act & Assert

            await Assert.ThrowsAsync<EsException>(() => _lcTool.PutAsync(invalidPolicyRequest));
        }
    }
}
