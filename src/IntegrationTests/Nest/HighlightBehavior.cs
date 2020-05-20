using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using Nest;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Nest
{
    public class HighlightBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        private readonly ITestOutputHelper _output;

        public HighlightBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
        }

        [Fact]
        public async Task ShouldHighlightFoundText()
        {
            //Arrange
            IHit<TestEntity> hit = null;
            KeyValuePair<string, IReadOnlyCollection<string>> highlight = default;
            int highlightCount = 0;

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                var indexResponse = await _clFx.EsClient.IndexManyAsync(
                    new[] {
                        new TestEntity { Id = 10, Value = "some foo text here. id=10" },
                        new TestEntity { Id = 1, Value = "some bar string here. id=1" }
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
                                        .Query("foo text")))
                            .Highlight(h => h
                                .PreTags("<b>")
                                .PostTags("</b>")
                                .Encoder(HighlighterEncoder.Html)
                                .Fields(d => 
                                    d.Field(entity => entity.Value)
                                        .HighlightQuery(qd => 
                                            qd.Match(qdm => qdm
                                                .Field(entity => entity.Value)
                                                .Query("foo text 10"))))));

                    hit = sr.Hits.First();
                    highlight = hit.Highlight.First();
                    highlightCount = hit.Highlight.Count;
                }
            });

            var str = JsonConvert.SerializeObject(hit, Formatting.Indented);
            _output.WriteLine("");
            _output.WriteLine("HIT:");
            _output.WriteLine(str);

            //Assert
            Assert.Equal("val", highlight.Key);
            Assert.Equal(1, highlight.Value.Count);
            Assert.Contains("some <b>foo</b> <b>text</b> here. id=<b>10</b>", highlight.Value);
        }
    }
}
