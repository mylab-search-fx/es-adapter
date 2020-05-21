using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Provides ElasticSearch NEST client
    /// </summary>

    public interface IEsManager
    {
        /// <summary>
        /// ES NEST client
        /// </summary>
        ElasticClient Client { get; }
    }
}
