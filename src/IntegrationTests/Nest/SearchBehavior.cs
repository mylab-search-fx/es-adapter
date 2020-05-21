using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using Nest;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Nest
{
    public class SearchBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        private readonly ITestOutputHelper _output;

        public SearchBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
        }

        [Fact]
        public async Task ShouldFindById()
        {
            //Arrange 
            IHit<TestEntity> hit = null;

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                var indexResponse = await _clFx.EsClient.IndexAsync(
                    new TestEntity
                    {
                        Id = 10,
                        Value = "foo"
                    }, indD => indD.Index(indNm));

                _output.WriteLine("Index response: \n" + indexResponse.DebugInformation);

                await Task.Delay(1000);

                if (indexResponse.IsValid)
                {
                    //Act
                    var sr = await _clFx.EsClient.SearchAsync<TestEntity>(sd => 
                        sd.Index(indNm)
                            .Query(q => q
                            .Ids(s => 
                                s.Values(10))));

                    hit = sr.Hits?.FirstOrDefault();
                }
            });

            if (hit != null)
            {
                var str = JsonConvert.SerializeObject(hit, Formatting.Indented);
                _output.WriteLine("");
                _output.WriteLine("HIT:");
                _output.WriteLine(str);
            }

            //Assert
            Assert.NotNull(hit);
            Assert.Equal(10, hit.Source.Id);
            Assert.Equal("foo", hit.Source.Value);
        }

        [Fact]
        public async Task ShouldFindByField()
        {
            //Arrange
            IHit<TestEntity> hit = null;
            int hitCount = 0;

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                var indexResponse = await _clFx.EsClient.IndexManyAsync(
                    new [] {
                        new TestEntity { Id = 10, Value = "foo" },
                        new TestEntity { Id = 1, Value = "bar" }
                    }, 
                    indNm);

                _output.WriteLine("Index response: \n" + indexResponse.DebugInformation);

                await Task.Delay(1000);

                if (indexResponse.IsValid)
                {
                    //Act
                    var sr = await _clFx.EsClient.SearchAsync<TestEntity>(sd =>
                        sd.Index(indNm)
                            .Query(q => q
                                .Range(r =>
                                    r.Field(ent => ent.Id)
                                        .GreaterThan(2))));

                    hit = sr.Hits?.FirstOrDefault();
                    hitCount = sr.Hits?.Count ?? 0;
                }
            });

            if (hit != null)
            {
                var str = JsonConvert.SerializeObject(hit, Formatting.Indented);
                _output.WriteLine("");
                _output.WriteLine("HIT:");
                _output.WriteLine(str);
            }

            //Assert
            Assert.Equal(1, hitCount);
            Assert.NotNull(hit);
            Assert.Equal(10, hit.Source.Id);
            Assert.Equal("foo", hit.Source.Value);
        }

        [Fact]
        public async Task ShouldFindByFullText()
        {
            //Arrange
            IHit<TestEntity> hit = null;
            int hitCount = 0;

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                var indexResponse = await _clFx.EsClient.IndexManyAsync(
                    new[] {
                        new TestEntity { Id = 10, Value = "some foo text here" },
                        new TestEntity { Id = 1, Value = "some bar string here" }
                    },
                    indNm);

                _output.WriteLine("Index response: \n" + indexResponse.DebugInformation);

                await Task.Delay(1000);

                if (indexResponse.IsValid)
                {
                    //Act
                    var sr = await _clFx.EsClient.SearchAsync<TestEntity>(sd =>
                        sd.Index(indNm)
                            .Query(q => q
                                .Match(r =>
                                    r.Field(ent => ent.Value)
                                        .Query("foo text"))));

                    hit = sr.Hits?.FirstOrDefault();
                    hitCount = sr.Hits?.Count ?? 0;
                }
            });

            if (hit != null)
            {
                var str = JsonConvert.SerializeObject(hit, Formatting.Indented);
                _output.WriteLine("");
                _output.WriteLine("HIT:");
                _output.WriteLine(str);
            }

            //Assert
            Assert.Equal(1, hitCount);
            Assert.NotNull(hit);
            Assert.Equal(10, hit.Source.Id);
            Assert.Equal("some foo text here", hit.Source.Value);
        }
    }
}
