using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Provides object model for searching 
    /// </summary>
    public class EsSearchEngine<TDoc> : IEsSearchEngine<TDoc> where TDoc : class
    {
        private readonly string _indexName;
        private readonly IEsSearcher<TDoc> _searcher;
        private readonly IEsSearchEngineStrategy<TDoc> _defaultStrategy;

        private readonly IDictionary<string, IEsSearchFilter<TDoc>> _registeredFilters =
            new Dictionary<string, IEsSearchFilter<TDoc>>();

        private readonly IDictionary<string, IEsSearchSort<TDoc>> _registeredSorts 
            = new Dictionary<string, IEsSearchSort<TDoc>>();

        /// <summary>
        /// Initializes a new instance of <see cref="SearchEngine"/>
        /// </summary>
        public EsSearchEngine(IIndexNameProvider indexNameProvider, IEsSearcher<TDoc> searcher, IEsSearchEngineStrategy<TDoc> defaultStrategy)
        {
            _indexName = indexNameProvider.Provide<TDoc>();
            _searcher = searcher ?? throw new ArgumentNullException(nameof(searcher));
            _defaultStrategy = defaultStrategy;
        }

        /// <summary>
        /// Performs searching
        /// </summary>
        public async Task<EsFound<TDoc>> SearchAsync(
            string queryStr, 
            IEsSearchEngineStrategy<TDoc> strategy = null, 
            string filterKey = null, 
            string sortKey = null, 
            EsPaging paging = null,
            CancellationToken cancellationToken = default)
        {
            var sp = new SearchParams<TDoc>(d => CreateSearchQuery(d, queryStr, filterKey, strategy))
            {
                Page = paging
            };

            if (!string.IsNullOrWhiteSpace(sortKey))
                sp.Sort = GetSort(sortKey);

            return await _searcher.ForIndex(_indexName).SearchAsync(sp, cancellationToken);
        }

        /// <summary>
        /// Registers filter with specified key
        /// </summary>
        protected void RegisterFilter(string key, IEsSearchFilter<TDoc> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            _registeredFilters.Add(key, filter);
        }

        /// <summary>
        /// Registers sort with specified key
        /// </summary>
        protected void RegisterSort(string key, IEsSearchSort<TDoc> sort)
        {
            if (sort == null) throw new ArgumentNullException(nameof(sort));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            _registeredSorts.Add(key, sort);
        }

        private QueryContainer CreateSearchQuery(QueryContainerDescriptor<TDoc> d, string queryStr, string filterKey, IEsSearchEngineStrategy<TDoc> strategy)
        {
            var actualStrategy = strategy ?? _defaultStrategy;

            if(actualStrategy == null)
                throw new InvalidOperationException("Search strategy not specified");

            var propSearch = GetPropertySearch(queryStr, actualStrategy);

            var filters = new List<Func<QueryContainerDescriptor<TDoc>, QueryContainer>>(GetFilters(queryStr, actualStrategy));

            if (!string.IsNullOrWhiteSpace(filterKey))
            {
                var registeredFilter = GetFilter(filterKey);
                if (registeredFilter != null)
                    filters.Add(registeredFilter);
            }

            return d.Bool(boolSd =>
            {
                var cs = boolSd;

                cs = cs.Should(propSearch);

                cs = cs.Filter(filters);

                return cs;

            });
        }

        IEnumerable<Func<QueryContainerDescriptor<TDoc>, QueryContainer>> GetPropertySearch(string queryStr, IEsSearchEngineStrategy<TDoc> strategy)
        {
            var searchFuncs = new List<Func<QueryContainerDescriptor<TDoc>, QueryContainer>>();

            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                var termProps = strategy.GetTermProperties().ToArray();
                searchFuncs.Add(d =>
                    d.MultiMatch(mm =>
                        mm.Fields(termProps).Query(queryStr)));

                var words = queryStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    if(long.TryParse(word, out var longWord))
                    {
                        foreach (var numProperty in strategy.GetNumProperties())
                        {
                            searchFuncs.Add(d =>
                                d.Term(p =>
                                    p.Field(numProperty).Value(longWord)
                                )
                            );
                        }
                    }

                    foreach (var termProperty in strategy.GetTermProperties())
                    {
                        searchFuncs.Add(d =>
                            d.Prefix(p =>
                                p.Field(termProperty).Value(word)
                            )
                        );
                    }

                    foreach (var textProperty in strategy.GetTextProperties())
                    {
                        searchFuncs.Add(d =>
                            d.Prefix(matchD =>
                                matchD.Field(textProperty).Value(word)
                            )
                        );
                    }
                }
            }
            else
            {
                searchFuncs.Add(d => d.MatchAll());
            }

            return searchFuncs;
        }

        IEnumerable<Func<QueryContainerDescriptor<TDoc>, QueryContainer>> GetFilters(string queryStr, IEsSearchEngineStrategy<TDoc> strategy)
        {
            var fieldTermsList = new List<Func<QueryContainerDescriptor<TDoc>, QueryContainer>>();

            fieldTermsList.AddRange(strategy
                .GetPredefinedFilters()
                .Select<IEsSearchFilter<TDoc>, Func<QueryContainerDescriptor<TDoc>, QueryContainer>>(filter => filter.Filter));

            fieldTermsList.AddRange(strategy
                .GetFiltersFromQuery(queryStr)
                .Select<IEsSearchFilter<TDoc>, Func<QueryContainerDescriptor<TDoc>, QueryContainer>>(filter => filter.Filter));

            return fieldTermsList;
        }

        private Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> GetSort(string sortKey)
        {
            if (_registeredSorts == null || !_registeredSorts.TryGetValue(sortKey, out var foundSort))
                throw new NotSupportedException($"Sort not supported: '{sortKey}'");

            return foundSort.Sort;
        }

        private Func<QueryContainerDescriptor<TDoc>, QueryContainer> GetFilter(string filterKey)
        {
            if (_registeredFilters == null || !_registeredFilters.TryGetValue(filterKey, out var foundFilter))
                throw new NotSupportedException($"Filter not supported: '{filterKey}'");

            return foundFilter.Filter;
        }
    }
}
