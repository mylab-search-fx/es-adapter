using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.SearchEngine
{
    public partial class EsSearchEngine<TDoc>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SearchEngine"/>
        /// </summary>
        public EsSearchEngine(IIndexNameProvider indexNameProvider, IEsSearcher<TDoc> searcher,
            IEsSearchEngineStrategy<TDoc> defaultStrategy)
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
            else
            {
                var defaultSort = strategy?.DefaultSort ?? DefaultSort;
                if (defaultSort != null)
                    sp.Sort = defaultSort.Sort;
            }

            return await _searcher.ForIndex(_indexName).SearchAsync(sp, cancellationToken);
        }
    }
}