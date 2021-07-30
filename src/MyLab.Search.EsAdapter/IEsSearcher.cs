using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter
{

    /// <summary>
    /// Performs searches
    /// </summary>
    public interface IEsSearcher<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Search documents in specified index
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync(string indexName,SearchParams<TDoc> searchParams, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search documents in index which bound to document model
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search documents with highlighting in specified index
        /// </summary>
        Task<EsHlFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search documents with highlighting in index which bound to document model
        /// </summary>
        Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector, CancellationToken cancellationToken = default);  

        /// <summary>
        /// Create index specific searcher
        /// </summary>
        IIndexSpecificEsSearcher<TDoc> ForIndex(string indexName);

        /// <summary>
        /// Create searcher for default index
        /// </summary>
        IIndexSpecificEsSearcher<TDoc> ForDefaultIndex();
    }

    /// <summary>
    /// Performs searches in specific index
    /// </summary>
    public interface IIndexSpecificEsSearcher<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Gets specific index name
        /// </summary>
        public string IndexName { get; }

        /// <summary>
        /// Search documents in index which bound to document model
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search documents with highlighting in index which bound to document model
        /// </summary>
        Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector, CancellationToken cancellationToken = default);
    }
}
