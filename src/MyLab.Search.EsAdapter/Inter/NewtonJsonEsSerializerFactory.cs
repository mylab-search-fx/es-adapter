using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Creates NewtonJson serializer 
    /// </summary>
    public class NewtonJsonEsSerializerFactory : IEsSerializerFactory
    {
        /// <inheritdoc />
        public IElasticsearchSerializer Create(IElasticsearchSerializer builtin, IConnectionSettingsValues settings)
        {
            return new NewtonJsonEsSerializer();
        }
    }
}