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
    public class EsComponentTemplateToolsBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsComponentTemplateTools _ctTools;
        private readonly string _cTemplateExampleJson;

        public EsComponentTemplateToolsBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            var esClientProvider = new SingleEsClientProvider(client);

            _ctTools = new EsComponentTemplateTools(esClientProvider);

            _cTemplateExampleJson = File.ReadAllText("Files\\component-template-example.json");
        }

        [Fact]
        public async Task ShouldCreateLifecycle()
        {
            //Arrange
            var ctId = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _ctTools.PutComponentTemplateAsync(ctId, _cTemplateExampleJson, CancellationToken.None);
            var streamExists = await _ctTools.IsComponentTemplateExistsAsync(ctId);

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldDeleteLifecycle()
        {
            //Arrange
            var ctId = Guid.NewGuid().ToString("N");
            await _ctTools.PutComponentTemplateAsync(ctId, _cTemplateExampleJson, CancellationToken.None);

            //Act
            await _ctTools.DeleteComponentTemplateAsync(ctId);

            var streamExists = await _ctTools.IsComponentTemplateExistsAsync(ctId);

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentLifecycle()
        {
            //Arrange
            var randomId = Guid.NewGuid().ToString("N");

            //Act
            var exists = await _ctTools.IsComponentTemplateExistsAsync(randomId);

            //Assert
            Assert.False(exists);
        }
    }
}
