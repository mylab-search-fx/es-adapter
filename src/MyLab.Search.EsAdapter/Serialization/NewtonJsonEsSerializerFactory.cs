using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsAdapter.Serialization
{
    /// <summary>
    /// Creates NewtonJson serializer 
    /// </summary>
    public class NewtonJsonEsSerializerFactory : IEsSerializerFactory
    {
        public IElasticsearchSerializer Create(IElasticsearchSerializer builtin, IConnectionSettingsValues settings)
        {
            return new NewtonJsonEsSerializer();
        }
    }
}