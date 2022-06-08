using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Search;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class GenericEsSearcherBehavior :
        IClassFixture<TestPreindexEsFixture>,
        IClassFixture<TestClientFixture>
    {
        private readonly EsSearcher<TestDoc> _searcher;

        public GenericEsSearcherBehavior(
            TestClientFixture fxt,
            TestPreindexEsFixture preindexFxt,
            ITestOutputHelper output)
        {
            var client = fxt.GetClientProvider(output);
            var esClientProvider = new SingleEsClientProvider(client);
            
            var options = new EsOptions
            {
                IndexBindings = new[]
                {
                    new IndexBinding
                    {
                        Doc = "foo",
                        Index = preindexFxt.IndexName
                    }
                }
            };

            var baseSearcher = new EsSearcher(esClientProvider);

            _searcher = new EsSearcher<TestDoc>(baseSearcher, options);
        }

        [Fact]
        public async Task ShouldFindWithQuery()
        {
            //Arrange
            var request = new EsSearchParams<TestDoc>(d => d.Ids(ids => ids.Values(0,4)));

            //Act
            var found = await _searcher.SearchAsync(request);

            var foundDoc0 = found.FirstOrDefault(d => d.Id == "0");
            var foundDoc1 = found.FirstOrDefault(d => d.Id == "4");

            //Assert
            Assert.Equal(2, found.Count);
            Assert.Equal(2, found.Total);

            Assert.NotNull(foundDoc0);
            Assert.Equal("foo-content-0", foundDoc0.Content);

            Assert.NotNull(foundDoc1);
            Assert.Equal("bar-content-4", foundDoc1.Content);
        }

        [Fact]
        public async Task ShouldPage()
        {
            //Arrange
            var request = new EsSearchParams<TestDoc>(d => d.MatchAll())
            {
                Sort = d => d.Ascending(doc => doc.Id),
                Paging = new EsPaging{From = 2, Size = 2}
            };

            //Act
            var found = await _searcher.SearchAsync(request);

            //Assert
            Assert.Equal(2, found.Count);
            Assert.Equal(8, found.Total);
            Assert.Contains(found, d => d.Id == "2");
            Assert.Contains(found, d => d.Id == "3");
        }

        [Theory]
        [InlineData(SortOrder.Ascending, "0")]
        [InlineData(SortOrder.Descending, "7")]
        public async Task ShouldSort(SortOrder sortOrder, string foundId)
        {
            //Arrange
            var request = new EsSearchParams<TestDoc>(d => d.MatchAll())
            {
                Sort = d => d.Field(doc => doc.Id, sortOrder),
                Paging = new EsPaging { From = 0, Size = 1 }
            };

            //Act
            var found = await _searcher.SearchAsync(request);

            //Assert
            Assert.Single(found);
            Assert.Equal(8, found.Total);
            Assert.Contains(found, d => d.Id == foundId);
        }

        [Fact]
        public async Task ShouldHighlight()
        {
            //Arrange
            var request = new EsSearchParams<TestDoc>(d => d.Ids(ids => ids.Values(0)));
            
            var highlight = new EsHlSelector<TestDoc>(d => d
                .HighlightQuery(q => q
                    .Match(mq => mq.Query("content").Field(doc => doc.Content)))
                .Fields(fd => fd.Field(doc => doc.Content))
            );

            string foundHlValue = null;

            //Act
            var found = await _searcher.SearchAsync(request, highlight);
            var foundHl = found.FirstOrDefault()?.Highlights;
            foundHl?.TryGetValue("content", out foundHlValue);

            //Assert
            Assert.Equal("foo-<em>content</em>-0", foundHlValue);
        }
    }
}
