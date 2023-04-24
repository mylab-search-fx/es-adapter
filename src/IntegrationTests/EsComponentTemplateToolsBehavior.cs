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
        private readonly string _cTemplateExample2Json;

        public EsComponentTemplateToolsBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            var esClientProvider = new SingleEsClientProvider(client);

            _ctTools = new EsComponentTemplateTools(esClientProvider);

            _cTemplateExampleJson = File.ReadAllText("Files\\component-template-example.json");
            _cTemplateExample2Json = File.ReadAllText("Files\\component-template-example-2.json");
        }

        [Fact]
        public async Task ShouldCreateLifecycle()
        {
            //Arrange
            var templateName = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _ctTools.PutComponentTemplateAsync(templateName, _cTemplateExampleJson, CancellationToken.None);
            var streamExists = await _ctTools.IsComponentTemplateExistentAsync(templateName, CancellationToken.None);

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldGetTemplate()
        {
            //Arrange
            var templateName = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _ctTools.PutComponentTemplateAsync(templateName, _cTemplateExampleJson, CancellationToken.None);
            var cTemplate = await _ctTools.TryGetComponentTemplateAsync(templateName, CancellationToken.None);

            //Assert
            Assert.NotNull(cTemplate);
            Assert.Contains(cTemplate.Meta, m => m.Key == "ver" && (string)m.Value == "1");
        }

        [Fact]
        public async Task ShouldUpdateTemplate()
        {
            //Arrange
            var templateName = Guid.NewGuid().ToString("N");

            //Act
            await using var deleter = await _ctTools.PutComponentTemplateAsync(templateName, _cTemplateExampleJson, CancellationToken.None);
            await _ctTools.PutComponentTemplateAsync(templateName, _cTemplateExample2Json, CancellationToken.None);
            var cTemplate = await _ctTools.TryGetComponentTemplateAsync(templateName, CancellationToken.None);

            //Assert
            Assert.NotNull(cTemplate);
            Assert.Contains(cTemplate.Meta, m => m.Key == "ver" && (string)m.Value == "2");
        }

        [Fact]
        public async Task ShouldDeleteLifecycle()
        {
            //Arrange
            var templateName = Guid.NewGuid().ToString("N");
            await _ctTools.PutComponentTemplateAsync(templateName, _cTemplateExampleJson, CancellationToken.None);

            //Act
            await _ctTools.DeleteComponentTemplateAsync(templateName, CancellationToken.None);

            var streamExists = await _ctTools.IsComponentTemplateExistentAsync(templateName, CancellationToken.None);

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentLifecycle()
        {
            //Arrange
            var randomName = Guid.NewGuid().ToString("N");

            //Act
            var exists = await _ctTools.IsComponentTemplateExistentAsync(randomName, CancellationToken.None);

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldGetNullInfoAboutAbsentTemplate()
        {
            //Arrange
            var randomName = Guid.NewGuid().ToString("N");

            //Act
            var cTemplate = await _ctTools.TryGetComponentTemplateAsync(randomName, CancellationToken.None);

            //Assert
            Assert.Null(cTemplate);
        }
    }
}
