namespace MyLab.Search.EsAdapter.SearchEngine
{
    /// <summary>
    /// Extracts filter from query
    /// </summary>
    public interface IEsSearchQueryFilterExtractor<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Extracts filter from query word
        /// </summary>
        IEsSearchFilter<TDoc> CreateEsSearchFilter(string queryWord);
    }
}