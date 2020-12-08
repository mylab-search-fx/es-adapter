using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEsSearchEngineStrategy<TDoc> _strategy;
        private readonly IEsSearcher<TDoc> _searcher;

        private readonly IDictionary<string, IEsSearchFilter<TDoc>> _registeredFilters =
            new Dictionary<string, IEsSearchFilter<TDoc>>();

        private readonly IDictionary<string, IEsSearchSort<TDoc>> _registeredSorts 
            = new Dictionary<string, IEsSearchSort<TDoc>>();

        /// <summary>
        /// Initializes a new instance of <see cref="SearchEngine"/>
        /// </summary>
        public EsSearchEngine(IIndexNameProvider indexNameProvider, IEsSearcher<TDoc> searcher, IEsSearchEngineStrategy<TDoc> strategy)
        {
            _indexName = indexNameProvider.Provide<TDoc>();
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _searcher = searcher ?? throw new ArgumentNullException(nameof(searcher));
        }

        /// <summary>
        /// Performs searching
        /// </summary>
        public async Task<EsFound<TDoc>> SearchAsync(string queryStr, string filterKey = null, string sortKey = null, EsPaging paging = null)
        {
            var sp = new SearchParams<TDoc>(d => CreateSearchQuery(d, queryStr, filterKey))
            {
                Sort = GetSort(sortKey),
                Page = paging
            };

            return await _searcher.ForIndex(_indexName).SearchAsync(sp);
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

        private QueryContainer CreateSearchQuery(QueryContainerDescriptor<TDoc> d, string queryStr, string filterKey)
        {
            var propSearch = GetPropertySearch(queryStr);

            var filters = new List<Func<QueryContainerDescriptor<TDoc>, QueryContainer>>(GetFilters(queryStr));

            var registeredFilter = GetFilter(filterKey);
            if(registeredFilter != null)
                filters.Add(registeredFilter);

            return d.Bool(boolSd =>
            {
                var cs = boolSd;

                cs = cs.Should(propSearch).MinimumShouldMatch(1);

                cs = cs.Filter(filters);

                return cs;

            });
        }

        IEnumerable<Func<QueryContainerDescriptor<TDoc>, QueryContainer>> GetPropertySearch(string queryStr)
        {
            var fieldTermsList = new List<Func<QueryContainerDescriptor<TDoc>, QueryContainer>>();

            foreach (var termProperty in _strategy.GetTermProperties())
            {
                fieldTermsList.Add(d => 
                    d.Match(matchD => 
                        matchD.Field(termProperty).Query(queryStr)
                        )
                    );
            }

            foreach (var textProperty in _strategy.GetTextProperties())
            {
                fieldTermsList.Add(d =>
                    d.Match(matchD =>
                        matchD.Field(textProperty).Query(queryStr)
                    )
                );
            }

            return fieldTermsList;
        }

        IEnumerable<Func<QueryContainerDescriptor<TDoc>, QueryContainer>> GetFilters(string queryStr)
        {
            var fieldTermsList = new List<Func<QueryContainerDescriptor<TDoc>, QueryContainer>>();

            fieldTermsList.AddRange(_strategy
                .GetPredefinedFilters()
                .Select<IEsSearchFilter<TDoc>, Func<QueryContainerDescriptor<TDoc>, QueryContainer>>(filter => filter.Filter));

            fieldTermsList.AddRange(_strategy
                .GetFiltersFromQuery(queryStr)
                .Select<IEsSearchFilter<TDoc>, Func<QueryContainerDescriptor<TDoc>, QueryContainer>>(filter => filter.Filter));

            return fieldTermsList;
        }

        private Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> GetSort(string sortKey)
        {
            if (sortKey == null || _registeredSorts == null || !_registeredSorts.TryGetValue(sortKey, out var foundSort))
                return null;

            return foundSort.Sort;
        }

        private Func<QueryContainerDescriptor<TDoc>, QueryContainer> GetFilter(string filterKey)
        {
            if (filterKey == null || _registeredFilters == null || !_registeredFilters.TryGetValue(filterKey, out var foundFilter))
                return null;

            return foundFilter.Filter;
        }
    }
}
