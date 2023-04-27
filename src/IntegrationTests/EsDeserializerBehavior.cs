using System.IO;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsDeserializerBehavior : IClassFixture<TestClientFixture>
    {
        private readonly IEsSerializer _tools;

        public EsDeserializerBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var clientProvider = new SingleEsClientProvider(fxt.Client);

            _tools = new EsSerializer(clientProvider);
        }

        [Fact]
        public void ShouldDeserializeLifecycle()
        {
            //Arrange
            using var jsonStream = File.OpenRead("Files\\lifecycle-example.json");

            //Act
            var policy = _tools.DeserializeLifecyclePolicy(jsonStream);

            //Assert
            Assert.NotNull(policy);
            Assert.Contains(policy.Policy.Meta, p => p.Key == "ver" && (string)p.Value == "1");
        }

        [Fact]
        public void ShouldDeserializeIndexTemplate()
        {
            //Arrange
            using var jsonStream = File.OpenRead("Files\\index-template-example.json");

            //Act
            var template = _tools.DeserializeIndexTemplate(jsonStream);

            //Assert
            Assert.NotNull(template);
            Assert.Contains(template.Meta, p => p.Key == "ver" && (string)p.Value == "1");
        }

        [Fact]
        public void ShouldDeserializeComponentTemplate()
        {
            //Arrange
            using var jsonStream = File.OpenRead("Files\\component-template-example.json");

            //Act
            var template = _tools.DeserializeComponentTemplate(jsonStream);

            //Assert
            Assert.NotNull(template);
            Assert.Contains(template.Meta, p => p.Key == "ver" && (string)p.Value == "1");
        }
    }
}
