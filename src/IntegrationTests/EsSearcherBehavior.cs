using System;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using MyLab.Elastic;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsSearcherBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        private readonly ITestOutputHelper _output;
        private readonly IEsSearcher<TestEntity> _searcher;
        private readonly IEsIndexer<TestEntity> _indexer;

        public EsSearcherBehavior(ClientFixture clFx, ITestOutputHelper output)
        {
            _clFx = clFx;
            _output = output;
            _searcher = new EsSearcher<TestEntity>(new SingleEsClientProvider(_clFx.EsClient));
            _indexer = new EsIndexer<TestEntity>(new SingleEsClientProvider(_clFx.EsClient));
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
                    await _indexer.IndexAsync(indNm, testEnt);
                    await Task.Delay(1000);

                    //Act
                    var res = await _searcher.SearchAsync(indNm, new SearchParams<TestEntity>(
                        
                        q =>
                                q.Ids(s => s.Values(10)))
                    );
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
                await _indexer.IndexAsync(indNm, testEnt);
                await Task.Delay(1000);

                //Act
                var res = await _searcher.SearchAsync(indNm, new SearchParams<TestEntity>(

                    q =>
                        q.Range(r =>
                            r.Field(ent => ent.Id)
                                .GreaterThan(2))

                    ));
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
                await _indexer.IndexManyAsync(indNm, testEnts);
                await Task.Delay(1000);

                //Act
                var res = await _searcher.SearchAsync(indNm, new SearchParams<TestEntity>(

                    q => q
                        .Match(r =>
                            r.Field(ent => ent.Value)
                                .Query("foo text"))

                    ));
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
                await _indexer.IndexManyAsync(indNm, items);
                await Task.Delay(1000);

                //Act
                var res = await _searcher.SearchAsync(indNm, new SearchParams<TestEntity>(

                        q => q
                            .Match(r =>
                                r.Field(ent => ent.Value)
                                    .Query("foo text"))

                        ),
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

        [Fact]
        public async Task ShouldSort()
        {
            //Arrange 
            TestEntity[] found = null;
            var testEntList = new []
            {
                new TestEntity{Id = 4, Value = "foo 3"},
                new TestEntity{Id = 3, Value = "foo 2"},
                new TestEntity{Id = 1, Value = "foo"}, 
                new TestEntity{Id = 2, Value = "foo 1"},
            };

            try
            {
                await _clFx.UseTmpIndexWithMap(async indNm =>
                {
                    await _indexer.IndexManyAsync(indNm, testEntList);
                    await Task.Delay(1000);

                    //Act
                    var foundColl = await _searcher.SearchAsync(indNm, new SearchParams<TestEntity>(

                        q =>
                            q.Match(m => m.Field("val").Query("foo"))

                        ));
                    found = foundColl.ToArray();

                });
            }
            catch (EsSearchException<TestEntity> e)
            {
                _output.WriteLine(e.Response.DebugInformation);
            }

            //Assert
            Assert.NotNull(found);
            Assert.Equal(testEntList.Length, found.Length);
            Assert.Equal("foo", found[0].Value);
            Assert.Equal("foo 1", found[1].Value);
            Assert.Equal("foo 2", found[2].Value);
            Assert.Equal("foo 3", found[3].Value);
        }
    }
}
