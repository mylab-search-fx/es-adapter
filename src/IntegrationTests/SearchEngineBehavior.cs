using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntegrationTests.Stuff;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Elastic;
using MyLab.Elastic.SearchEngine;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class SearchEngineBehavior 
    {
        private readonly ITestOutputHelper _output;

        private static readonly TestModel[] Models = GenerateTestModels();

        public SearchEngineBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ShouldSearchAllWhenQueryIsEmpty(string emptyQuery)
        {
            //Arrange
            

            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync(emptyQuery));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Select(m => m.Id).Take(10));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldFoundByTerms()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("term5"));

            //Assert
            Assert.NotNull(found);
            Assert.Single(found);

            Assert.Equal(5, found.First().Id);
        }

        [Fact]
        public async Task ShouldFoundByTextStart()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("text1"));

            //Assert

            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id == 1 || m.Id >= 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldFoundByTextStartAndTermStart()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("text1 term4", sortKey:"norm"));

            //Assert

            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id == 1 || m.Id == 4 || m.Id >= 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyRegisteredFilter()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("", "single-digit"));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id < 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyRegisteredSort()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("", sortKey:"revert"));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Reverse().Take(10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyPaging()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("", paging: new EsPaging
            {
                Size = 2,
                From = 3
            }));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Skip(3).Take(2).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyStrategyPredefinedFilter()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SearchEngineWithStrategyPredefinedFilter>(engine => engine.SearchAsync(""));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id < 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyQueryExtractedFilters()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("exclude5"));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id != 5).Select(m => m.Id).Take(10));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        async Task<EsFound<TestModel>> InvokeTest<TSearchEngine>(
            Func<IEsSearchEngine<TestModel>, Task<EsFound<TestModel>>> logic)
            where TSearchEngine : class, IEsSearchEngine<TestModel>
        {
            IServiceProvider sp = null;
            EsFound<TestModel> found = null;

            try
            {
                sp = StartSearchApp<TSearchEngine>();

                var searchEngine = sp.GetService<IEsSearchEngine<TestModel>>();
                var mgr = sp.GetService<IEsManager>();
                var indexer = sp.GetService<IEsIndexer<TestModel>>();

                await using (await mgr.CreateDefaultIndexAsync())
                {
                    await indexer.IndexManyAsync(Models);
                    await Task.Delay(1000);

                    //Act
                    found = await logic(searchEngine);
                } 
            }
            catch (EsSearchException<TestModel> e)
            {
                _output.WriteLine(e.Response.DebugInformation);
            }
            finally
            {

                CloseSearchApp(sp);
            }

            return found;
        }

        IServiceProvider StartSearchApp<TSearchEngine>()
            where TSearchEngine : class, IEsSearchEngine<TestModel>
        {
            var config = new ConfigurationBuilder().Build();
            var srvCollection = new ServiceCollection();
            srvCollection.AddEsTools(config);
            srvCollection.AddEsSearchEngine<TSearchEngine, TestModel>();
            srvCollection.Configure<ElasticsearchOptions>(o =>
            {
                o.Url = "http://localhost:10115";
                o.DefaultIndex = "test-index-" + Guid.NewGuid().ToString("N");
            });
            srvCollection.AddSingleton<IEsClientProvider>(
                new TestEsClientProvider("http://localhost:10115", _output));

            return srvCollection.BuildServiceProvider();
        }

        void CloseSearchApp(IServiceProvider serviceProvider)
        {
            var clientProvider = serviceProvider?.GetService<IEsClientProvider>();
            (clientProvider as IDisposable)?.Dispose();
        }

        static TestModel[] GenerateTestModels()
        {
            return Enumerable
                .Range(0, 15)
                .Select(i => new TestModel
                {
                    Id = i,
                    Term = "term" + i,
                    Text = "text" + i
                }).ToArray();
        }

        [ElasticsearchType(IdProperty = nameof(Id))]
        public class TestModel
        {
            [Number(NumberType.Integer, Name = "uid")]
            public int Id { get; set; }
            [Keyword(Name = "trm")]
            public string Term { get; set; }
            [Text(Name = "txt")]
            public string Text { get; set; }
        }

        class WithPredefinedFilterSearchStrategy : EsSearchEngineStrategy<TestModel>
        {
            public WithPredefinedFilterSearchStrategy()
            {
                AddTermProperty(entity => entity.Term);
                AddTextProperty(entity => entity.Text);
                AddPredefinedFilter(new SingleDigitTermFilter());
            }
        }

        class SearchEngineWithStrategyPredefinedFilter : EsSearchEngine<TestModel>
        {
            public SearchEngineWithStrategyPredefinedFilter(
                IIndexNameProvider indexNameProvider,
                IEsSearcher<TestModel> searcher)
                : base(indexNameProvider, searcher, new WithPredefinedFilterSearchStrategy())
            {
            }
        }

        class SimpleSearchStrategy : EsSearchEngineStrategy<TestModel>
        {
            public SimpleSearchStrategy()
            {
                AddTermProperty(entity => entity.Term);
                AddTextProperty(entity => entity.Text);
                AddFilterExtractor(new NumberExcludeFilterFilterExtractor());
            }
        }

        class SimpleSearchEngine : EsSearchEngine<TestModel>
        {
            public SimpleSearchEngine(
                IIndexNameProvider indexNameProvider, 
                IEsSearcher<TestModel> searcher) 
                : base(indexNameProvider, searcher, new SimpleSearchStrategy())
            {
                RegisterFilter("single-digit", new SingleDigitTermFilter());
                RegisterSort("revert", new RevertSort());
                RegisterSort("norm", new NormSort());
            }
        }

        class SingleDigitTermFilter : IEsSearchFilter<TestModel>
        {
            public QueryContainer Filter(QueryContainerDescriptor<TestModel> d)
                => d.Wildcard(m => m.Term, "term?");
        }

        class RevertSort : IEsSearchSort<TestModel>
        {
            public IPromise<IList<ISort>> Sort(SortDescriptor<TestModel> d)
                => d.Field(m => m.Id, SortOrder.Descending);
        }

        class NormSort : IEsSearchSort<TestModel>
        {
            public IPromise<IList<ISort>> Sort(SortDescriptor<TestModel> d)
                => d.Field(m => m.Id, SortOrder.Ascending);
        }

        class NumberExcludeFilterFilterExtractor : EsSearchQueryFilterExtractor<TestModel>
        {
            public NumberExcludeFilterFilterExtractor() : base("exclude(?<digit>\\d{1})")
            {
            }

            protected override IEsSearchFilter<TestModel> CreateFilter(Match queryMatch)
            {
                var i = int.Parse(queryMatch.Groups["digit"].Value);

                return new NumberExcludeFilter(i);
            }

            class NumberExcludeFilter : IEsSearchFilter<TestModel>
            {
                private readonly int _i;

                public NumberExcludeFilter(int i)
                {
                    _i = i;
                }
                public QueryContainer Filter(QueryContainerDescriptor<TestModel> d)
                    => !d.Term(t => t.Field(m => m.Term).Value("term" + _i));
            }
        }
    }
}
