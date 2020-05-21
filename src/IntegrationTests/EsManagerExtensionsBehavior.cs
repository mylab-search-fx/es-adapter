using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using MyLab.Elastic;
using Nest;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsManagerExtensionsBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        private readonly ITestOutputHelper _output;
        private readonly IEsManager _mgr;

        public EsManagerExtensionsBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
            _mgr = new TestEsManager(_clFx.EsClient);
        }

        [Fact]
        public async Task ShouldIndexDocument()
        {
            //Arrange
            var document = new TestEntity
            {
                Id = 10,
                Value = "foo"
            };

            //Act & Assert
            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                await _mgr.IndexAsync(indNm,document);
            });
        }

        [Fact]
        public async Task ShouldNotIndexDocumentWhenIndexDoesNotExists()
        {
            //Arrange
            var document = new TestEntity
            {
                Id = 10,
                Value = "foo"
            };

            //Act & Assert
            await Assert.ThrowsAsync<EsIndexException>(() => _mgr.IndexAsync(" absent-index", document));
        }

        [Fact]
        public async Task ShouldFindById()
        {
            //Arrange 
            TestEntity found = null;
            var testEnt = new TestEntity{ Id = 10, Value = "foo" };

            try
            {
                await _clFx.UseTmpIndexWithMap(async indNm =>
                {
                    await _mgr.IndexAsync(indNm, testEnt);
                    await Task.Delay(1000);

                    //Act
                    var res = await _mgr.SearchAsync<TestEntity>(indNm, q =>
                        q.Ids(s => s.Values(10)));
                    found = res.FirstOrDefault();
                });
            }
            catch (EsSearchException<TestEntity> e)
            {
                _output.WriteLine(e.Response.DebugInformation);
            }

            //Assert
            Assert.NotNull(found);
            Assert.Equal(10, found.Id);
            Assert.Equal("foo", found.Value);
        }

        [Fact]
        public async Task ShouldFindByField()
        {
            //Arrange 
            TestEntity found = null;
            int foundCount = 0;
            var testEnt = new TestEntity { Id = 10, Value = "foo" };

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                Func<QueryContainerDescriptor<TestEntity>, QueryContainer> query = q => q
                    .Ids(s => s.Values(10));

                await _mgr.IndexAsync(indNm, testEnt);
                await Task.Delay(1000);

                //Act
                var res = await _mgr.SearchAsync<TestEntity>(indNm, q =>
                    q.Range(r =>
                        r.Field(ent => ent.Id)
                            .GreaterThan(2)));
                found = res.FirstOrDefault();
                foundCount = res.Count;
            });

            //Assert
            Assert.Equal(1, foundCount);
            Assert.NotNull(found);
            Assert.Equal(10, found.Id);
            Assert.Equal("foo", found.Value);
        }

        [Fact]
        public async Task ShouldFindByFullText()
        {
            //Arrange 
            TestEntity found = null;
            int foundCount = 0;
            var testEnts = new[]
            {
                new TestEntity {Id = 10, Value = "some foo text here"},
                new TestEntity {Id = 1, Value = "some bar string here"}
            };

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                await _mgr.IndexManyAsync(indNm, testEnts);
                await Task.Delay(1000);

                //Act
                var res = await _mgr.SearchAsync<TestEntity>(indNm, q => q
                    .Match(r =>
                        r.Field(ent => ent.Value)
                            .Query("foo text")));
                found = res.FirstOrDefault();
                foundCount = res.Count;
            });

            //Assert
            Assert.Equal(1, foundCount);
            Assert.NotNull(found);
            Assert.Equal(10, found.Id);
            Assert.Equal("some foo text here", found.Value);
        }

        [Fact]
        public async Task ShouldHighlightFoundText()
        {
            //Arrange
            HighLightedDocument<TestEntity> doc = null;
            int highlightCount = 0;
            var items = new[]
            {
                new TestEntity {Id = 10, Value = "some foo text here. id=10"},
                new TestEntity {Id = 1, Value = "some bar string here. id=1"}
            };

            await _clFx.UseTmpIndexWithMap(async indNm =>
            {
                await _mgr.IndexManyAsync(indNm, items);
                await Task.Delay(1000);

                //Act
                var res = await _mgr.SearchAsync<TestEntity>(indNm, q => q
                    .Match(r =>
                        r.Field(ent => ent.Value)
                            .Query("foo text")),
                    h => h
                        .PreTags("<b>")
                        .PostTags("</b>")
                        .Encoder(HighlighterEncoder.Html)
                        .Fields(d =>
                            d.Field(entity => entity.Value)
                                .HighlightQuery(qd =>
                                    qd.Match(qdm => qdm
                                        .Field(entity => entity.Value)
                                        .Query("foo text 10"))))
                    );

                doc = res.FirstOrDefault();
                highlightCount = res.Count;
            });
            
            //Assert
            Assert.Equal(1, highlightCount);
            Assert.Contains("some <b>foo</b> <b>text</b> here. id=<b>10</b>", doc.Highlight);
        }
    }
}
