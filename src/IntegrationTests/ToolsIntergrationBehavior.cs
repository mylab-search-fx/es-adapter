using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Search.EsAdapter;
using Xunit;

namespace IntegrationTests
{
    public class ToolsIntegrationBehavior
    {
        [Fact]
        public async Task ShouldIntegrate()
        {
            //Arrange
            var config = new ConfigurationBuilder().Build();

            //Act
            var serviceProvider = new ServiceCollection()
                .AddEsTools(config)
                .Configure<ElasticsearchOptions>(opt => opt.Url = "http://localhost:10115")
                .BuildServiceProvider();

            var testService = ActivatorUtilities.CreateInstance<TestService>(serviceProvider);
            var hasPing = await testService.HasPing();

            //Assert
            Assert.True(hasPing);
        }

        class TestService
        {
            private readonly IEsManager _es;

            public TestService(IEsManager es)
            {
                _es = es;
            }

            public Task<bool> HasPing()
            {
                return _es.PingAsync();
            }
        }
    }
}
