using System.Threading.Tasks;
using IntegrationTests.Stuff;
using MyLab.Elastic;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsManagerBehavior : IClassFixture<ClientFixture>
    {
        private readonly IEsManager _mgr;

        public EsManagerBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _mgr = new EsManager(new SingleEsClientProvider(clFx.EsClient));
        }

        [Fact]
        public async Task ShouldPing()
        {
            //Arrange
            

            //Act
            var hasPing = await _mgr.PingAsync();

            //Assert
            Assert.True(hasPing);
        }
    }
}
