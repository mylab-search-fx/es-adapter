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
    public class EsIndexTemplatesToolBehavior : IClassFixture<TestClientFixture>
    {
        private readonly EsIndexTemplatesTool _templatesTool;
        private readonly IEsIndexTemplateTool _testTemplate;
        private readonly string _testTemplateName;
        private readonly Func<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request> _putDescriptor;

        public EsIndexTemplatesToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var esClientProvider = new SingleEsClientProvider(fxt.Client);
            _templatesTool = new EsIndexTemplatesTool(esClientProvider);

            _testTemplateName = Guid.NewGuid().ToString("N");
            _testTemplate = new EsIndexTemplateTool(_testTemplateName, esClientProvider);

            _putDescriptor = d => d.IndexPatterns(Guid.NewGuid().ToString("N"));
        }

        [Fact]
        public async Task ShouldCreateIndexTemplateWithRequestObject()
        {
            //Arrange
            var putTemplateRequest = new PutIndexTemplateV2Request(_testTemplateName)
            {
                IndexPatterns = new[] { Guid.NewGuid().ToString("N") }
            };

            //Act
            await _templatesTool.PutAsync(putTemplateRequest);

            bool exists = await _testTemplate.ExistsAsync();

            if (exists)
            {
                await _testTemplate.DeleteAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldCreateIndexTemplateWithJsonRequest()
        {
            //Arrange
            var request = "{\"index_patterns\": \"foo\"}";

            //Act
            await _templatesTool.PutAsync(_testTemplateName, request);

            bool exists = await _testTemplate.ExistsAsync();

            if (exists)
            {
                await _testTemplate.DeleteAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldEnumerateIndexTemplates()
        {
            //Arrange
            
            await _testTemplate.PutAsync(_putDescriptor);

            //Act
            var indexes = await _templatesTool.GetAsync();
            var indexesArray = indexes.ToArray();

            //Assert
            Assert.Contains(indexesArray, a => a.Name == _testTemplateName);
        }

        [Fact]
        public async Task ShouldFilterIndexTemplates()
        {
            //Arrange
            await _testTemplate.PutAsync(_putDescriptor);

            //Act
            var indexes = await _templatesTool.GetAsync(d => d.Name(_testTemplateName));
            var indexesArray = indexes.ToArray();

            //Assert
            Assert.Contains(indexesArray, a => a.Name == _testTemplateName);
        }

        [Fact]
        public async Task ShouldDeleteIndexTemplatesByExactlyName()
        {
            //Arrange
            await _testTemplate.PutAsync(_putDescriptor);

            //Act
            await _templatesTool.DeleteByExactlyNamesAsync(new []{_testTemplateName});

            var exists = await _testTemplate.ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldDeleteIndexTemplatesByName()
        {
            //Arrange
            await _testTemplate.PutAsync(_putDescriptor);

            //Act
            await _templatesTool.DeleteByNameOrWildcardExpressionAsync(_testTemplateName);

            var exists = await _testTemplate.ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldDeleteIndexTemplatesByNamePattern()
        {
            //Arrange
            await _testTemplate.PutAsync(_putDescriptor);
            var nameTemplate = _testTemplateName.Remove(_testTemplateName.Length - 3) + "*";

            //Act
            await _templatesTool.DeleteByNameOrWildcardExpressionAsync(nameTemplate);

            var exists = await _testTemplate.ExistsAsync();

            //Assert
            Assert.False(exists);
        }
    }
}
