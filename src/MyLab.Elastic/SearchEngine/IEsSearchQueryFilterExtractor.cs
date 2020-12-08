using System.Collections.Generic;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Extracts filter from query
    /// </summary>
    public interface IEsSearchQueryFilterExtractor<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Extracts filter from query
        /// </summary>
        IEnumerable<IEsSearchFilter<TDoc>> CreateEsSearchFilters(string queryStr);
    }
}