using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Holds connection with ES
    /// </summary>
    public interface IEsClientProvider
    {
        /// <summary>
        /// Provides 
        /// </summary>
        ElasticClient Provide();
    }
}