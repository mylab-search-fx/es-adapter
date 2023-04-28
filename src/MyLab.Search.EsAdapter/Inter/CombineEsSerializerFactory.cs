using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Creates <see cref="CombineEsSerializer"/> serializer 
    /// </summary>
    public class CombineEsSerializerFactory : IEsSerializerFactory
    {
        /// <inheritdoc />
        public IElasticsearchSerializer Create(IElasticsearchSerializer builtin, IConnectionSettingsValues settings)
        {
            return new CombineEsSerializer(builtin);
        }
    }
}