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
    public class EsComponentTemplateToolBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsComponentTemplateTool _ctTool;
        private readonly string _cTemplateExampleJson;
        private readonly string _cTemplateExample2Json;
        private readonly SingleEsClientProvider _esClientProvider;
        private readonly string _templateName;

        public EsComponentTemplateToolBehavior(
            TestClientFixture clientFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            _esClientProvider = new SingleEsClientProvider(client);

            _templateName = Guid.NewGuid().ToString("N");

            _ctTool = new EsComponentTemplateTool(_templateName, _esClientProvider);

            _cTemplateExampleJson = File.ReadAllText("Files\\component-template-example.json");
            _cTemplateExample2Json = File.ReadAllText("Files\\component-template-example-2.json");
        }

        [Fact]
        public async Task ShouldCreateTemplate()
        {
            //Arrange

            //Act
            await using var deleter = await _ctTool.PutAsync(_cTemplateExampleJson);
            var streamExists = await _ctTool.ExistsAsync();

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldGetTemplate()
        {
            //Arrange

            //Act
            await using var deleter = await _ctTool.PutAsync(_cTemplateExampleJson);
            var cTemplate = await _ctTool.TryGetAsync();

            //Assert
            Assert.NotNull(cTemplate);
            Assert.Contains(cTemplate.Meta, m => m.Key == "ver" && (string)m.Value == "1");
        }

        [Fact]
        public async Task ShouldUpdateTemplate()
        {
            //Arrange

            //Act
            await using var deleter = await _ctTool.PutAsync(_cTemplateExampleJson);
            await _ctTool.PutAsync(_cTemplateExample2Json);
            var cTemplate = await _ctTool.TryGetAsync();

            //Assert
            Assert.NotNull(cTemplate);
            Assert.Contains(cTemplate.Meta, m => m.Key == "ver" && (string)m.Value == "2");
        }

        [Fact]
        public async Task ShouldDeleteTemplate()
        {
            //Arrange
            await _ctTool.PutAsync(_cTemplateExampleJson);

            //Act
            await _ctTool.DeleteAsync();

            var streamExists = await _ctTool.ExistsAsync();

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentTemplate()
        {
            //Arrange
            var randomName = Guid.NewGuid().ToString("N");

            //Act
            var exists = await new EsComponentTemplateTool(randomName, _esClientProvider).ExistsAsync(CancellationToken.None);

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldGetNullInfoAboutAbsentTemplate()
        {
            //Arrange
            var randomName = Guid.NewGuid().ToString("N");

            //Act
            var cTemplate = await new EsComponentTemplateTool(randomName, _esClientProvider).TryGetAsync(CancellationToken.None);

            //Assert
            Assert.Null(cTemplate);
        }

        [Fact]
        public async Task ShouldFailWhenEsErrorResponse()
        {
            //Arrange 
            var invalidTemplateRequest = new PutComponentTemplateRequest(_templateName);

            //Act & Assert

            await Assert.ThrowsAsync<EsException>(() => _ctTool.PutAsync(invalidTemplateRequest));
        }
    }
}
