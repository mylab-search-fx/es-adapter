using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Performs search requests
    /// </summary>
    public interface IEsSearcher<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Perform search request
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync(EsSearchParams<TDoc> searchParams,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Perform search request with highlight
        /// </summary>
        Task<EsHlFound<TDoc>> SearchAsync(EsSearchParams<TDoc> searchParams,
            EsHlSelector<TDoc> highlight, CancellationToken cancellationToken = default);
    }
}
