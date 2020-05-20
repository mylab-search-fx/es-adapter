using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Nest
{
    public class MappingBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        private readonly ITestOutputHelper _output;

        public MappingBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
        }

        [Fact]
        public async Task ShouldMapIndex()
        {
            //Act
            var mapResp = await _clFx.UseTmpIndex(
                indNm => _clFx.EsClient.Indices.GetMappingAsync(new GetMappingRequest(indNm)
                ),
                cd =>
                    cd.Map<TestEntity>(md => md.AutoMap()));

            _output.WriteLine(mapResp.ApiCall.DebugInformation);

            var indexMappings = mapResp.Indices.Values
                .FirstOrDefault()
                ?.Mappings;

            //Assert
            Assert.NotNull(indexMappings);
            Assert.Contains(indexMappings.Properties.Values, p => p.Name.Name == "uid" && p.Type == "integer");
            Assert.Contains(indexMappings.Properties.Values, p => p.Name.Name == "val" && p.Type == "text");
        }
    }
}
