using System;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Creates <see cref="CombineEsSerializer"/> serializer 
    /// </summary>
    public class CombineEsSerializerFactory : IEsSerializerFactory
    {
        private readonly JsonSerializer _jsonSerializer;

        /// <summary>
        /// Initializes a new instance of <see cref="CombineEsSerializerFactory"/>
        /// </summary>
        public CombineEsSerializerFactory()
            :this(new JsonSerializer())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CombineEsSerializerFactory"/>
        /// </summary>
        public CombineEsSerializerFactory(JsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        /// <inheritdoc />
        public IElasticsearchSerializer Create(IElasticsearchSerializer builtin, IConnectionSettingsValues settings)
        {
            return new CombineEsSerializer(builtin, _jsonSerializer);
        }
    }
}