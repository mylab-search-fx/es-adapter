using System.Threading.Tasks;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Represent search engine
    /// </summary>
    public interface IEsSearchEngine<TDoc> where TDoc : class
    {
        /// <summary>
        /// Performs searching
        /// </summary>
        Task<EsFound<TDoc>> SearchAsync(string queryStr, string filterKey = null, string sortKey = null, EsPaging paging = null);
    }
}