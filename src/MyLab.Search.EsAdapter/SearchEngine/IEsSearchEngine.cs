using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.SearchEngine
{
    /// <summary>
    /// Represent search engine
    /// </summary>
    public interface IEsSearchEngine<TDoc> where TDoc : class
    {
        /// <summary>
        /// Performs searching
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync(
            string queryStr, 
            IEsSearchEngineStrategy<TDoc> strategy = null, 
            string filterKey = null, 
            string sortKey = null, 
            EsPaging paging = null,
            CancellationToken cancellationToken = default);
    }
}