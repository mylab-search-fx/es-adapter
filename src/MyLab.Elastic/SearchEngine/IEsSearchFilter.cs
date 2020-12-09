using Nest;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Provides filter function
    /// </summary>
    public interface IEsSearchFilter<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// ES filter
        /// </summary>
        QueryContainer Filter(QueryContainerDescriptor<TDoc> d);
    }
}