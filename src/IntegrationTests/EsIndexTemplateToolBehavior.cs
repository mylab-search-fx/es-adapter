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
    public class EsIndexTemplateToolBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsIndexTemplateTool _itTool;
        private readonly string _cTemplateExampleJson;
        private readonly string _cTemplateExample2Json;
        private readonly SingleEsClientProvider _esClientProvider;
        private readonly string _templateName;

        public EsIndexTemplateToolBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            _esClientProvider = new SingleEsClientProvider(client);

            _templateName = nameof(EsIndexTemplateToolBehavior).ToLower() + "-" + Guid.NewGuid().ToString("N");

            _esClientProvider.Provide().Indices.DeleteTemplateV2(new DeleteIndexTemplateV2Request(nameof(EsIndexTemplateToolBehavior).ToLower() + "-*"));

            _itTool = new EsIndexTemplateTool(_templateName, _esClientProvider);

            _cTemplateExampleJson = File.ReadAllText("Files\\index-template-example.json");
            _cTemplateExample2Json = File.ReadAllText("Files\\index-template-example-2.json");
        }

        [Fact]
        public async Task ShouldCreateTemplate()
        {
            //Arrange

            //Act
            await using var deleter = await _itTool.PutAsync(_cTemplateExampleJson);
            var streamExists = await _itTool.ExistsAsync();

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldGetTemplate()
        {
            //Arrange

            //Act
            await using var deleter = await _itTool.PutAsync(_cTemplateExampleJson);
            var cTemplate = await _itTool.TryGetAsync();

            //Assert
            Assert.NotNull(cTemplate);
            Assert.Contains(cTemplate.Meta, m => m.Key == "ver" && (string)m.Value == "1");
        }

        [Fact]
        public async Task ShouldUpdateTemplate()
        {
            //Arrange

            //Act
            await using var deleter = await _itTool.PutAsync(_cTemplateExampleJson);
            await _itTool.PutAsync(_cTemplateExample2Json);
            var cTemplate = await _itTool.TryGetAsync();

            //Assert
            Assert.NotNull(cTemplate);
            Assert.Contains(cTemplate.Meta, m => m.Key == "ver" && (string)m.Value == "2");
        }

        [Fact]
        public async Task ShouldDeleteTemplate()
        {
            //Arrange
            await _itTool.PutAsync(_cTemplateExampleJson);

            //Act
            await _itTool.DeleteAsync();

            var streamExists = await _itTool.ExistsAsync();

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentTemplate()
        {
            //Arrange
            var randomName = Guid.NewGuid().ToString("N");

            //Act
            var exists = await new EsIndexTemplateTool(randomName, _esClientProvider).ExistsAsync(CancellationToken.None);

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldGetNullInfoAboutAbsentTemplate()
        {
            //Arrange
            var randomName = Guid.NewGuid().ToString("N");

            //Act
            var cTemplate = await new EsIndexTemplateTool(randomName, _esClientProvider).TryGetAsync(CancellationToken.None);

            //Assert
            Assert.Null(cTemplate);
        }
    }
}
