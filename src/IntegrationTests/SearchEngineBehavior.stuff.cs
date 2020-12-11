using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Elastic;
using MyLab.Elastic.SearchEngine;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class SearchEngineBehavior
    {
        private readonly ITestOutputHelper _output;
        private static readonly TestModel[] Models = GenerateTestModels();

        public SearchEngineBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        private async Task<EsFound<TestModel>> InvokeTest<TSearchEngine>(
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

        private IServiceProvider StartSearchApp<TSearchEngine>()
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

        private void CloseSearchApp(IServiceProvider serviceProvider)
        {
            var clientProvider = serviceProvider?.GetService<IEsClientProvider>();
            (clientProvider as IDisposable)?.Dispose();
        }

        private static TestModel[] GenerateTestModels()
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
            public long Id { get; set; }

            [Keyword(Name = "trm")] public string Term { get; set; }
            [Text(Name = "txt")] public string Text { get; set; }
        }

        private class WithPredefinedFilterSearchStrategy : EsSearchEngineStrategy<TestModel>
        {
            public WithPredefinedFilterSearchStrategy()
            {
                AddNumProperty(entity => entity.Id);
                AddTermProperty(entity => entity.Term);
                AddTextProperty(entity => entity.Text);
                AddPredefinedFilter(new SingleDigitTermFilter());
            }
        }

        private class SearchEngineWithStrategyPredefinedFilter : EsSearchEngine<TestModel>
        {
            public SearchEngineWithStrategyPredefinedFilter(
                IIndexNameProvider indexNameProvider,
                IEsSearcher<TestModel> searcher)
                : base(indexNameProvider, searcher, new WithPredefinedFilterSearchStrategy())
            {
            }
        }

        private class SimpleSearchStrategy : EsSearchEngineStrategy<TestModel>
        {
            public SimpleSearchStrategy()
            {
                AddNumProperty(entity => entity.Id);
                AddTermProperty(entity => entity.Term);
                AddTextProperty(entity => entity.Text);
                AddFilterExtractor(new NumberExcludeFilterFilterExtractor());
            }
        }

        private class SimpleSearchEngine : EsSearchEngine<TestModel>
        {
            public SimpleSearchEngine(
                IIndexNameProvider indexNameProvider,
                IEsSearcher<TestModel> searcher)
                : base(indexNameProvider, searcher, new SimpleSearchStrategy())
            {
                RegisterNamedFilter("single-digit", new SingleDigitTermFilter());
                RegisterNamedSort("revert", new RevertSort());
                RegisterNamedSort("norm", new NormSort());
            }
        }

        private class SingleDigitTermDefaultFilteredSearchEngine : EsSearchEngine<TestModel>
        {
            public SingleDigitTermDefaultFilteredSearchEngine(
                IIndexNameProvider indexNameProvider, 
                IEsSearcher<TestModel> searcher) 
                : base(indexNameProvider, searcher, new SimpleSearchStrategy())
            {
                DefaultFilter = new SingleDigitTermFilter();
            }
        }

        private class SingleDigitTermFilter : IEsSearchFilter<TestModel>
        {
            public QueryContainer Filter(QueryContainerDescriptor<TestModel> d)
                => d.Wildcard(m => m.Term, "term?");
        }

        private class RevertSort : IEsSearchSort<TestModel>
        {
            public IPromise<IList<ISort>> Sort(SortDescriptor<TestModel> d)
                => d.Field(m => m.Id, SortOrder.Descending);
        }

        private class NormSort : IEsSearchSort<TestModel>
        {
            public IPromise<IList<ISort>> Sort(SortDescriptor<TestModel> d)
                => d.Field(m => m.Id, SortOrder.Ascending);
        }

        private class NumberExcludeFilterFilterExtractor : EsSearchQueryFilterExtractor<TestModel>
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