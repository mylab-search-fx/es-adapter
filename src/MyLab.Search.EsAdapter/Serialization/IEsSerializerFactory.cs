using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsAdapter.Serialization
{
    /// <summary>
    /// User object serializer factory
    /// </summary>
    public interface IEsSerializerFactory
    {
        /// <summary>
        /// Creates user object serializer
        /// </summary>
        /// <param name="builtin">built-in serializer</param>
        /// <param name="settings">connection settings</param>
        IElasticsearchSerializer Create(IElasticsearchSerializer builtin, IConnectionSettingsValues settings);
    }
}
