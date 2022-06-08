using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Performs search requests
    /// </summary>
    public interface IEsSearcher
    {
        /// <summary>
        /// Perform search request
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync<TDoc>(string indexName, EsSearchParams<TDoc> searchParams, CancellationToken cancellationToken = default)
            where TDoc : class;

        /// <summary>
        /// Perform search request with highlight
        /// </summary>
        Task<EsHlFound<TDoc>> SearchAsync<TDoc>(string indexName, EsSearchParams<TDoc> searchParams, EsHlSelector<TDoc> highlight, CancellationToken cancellationToken = default)
            where TDoc : class;
    }
}
