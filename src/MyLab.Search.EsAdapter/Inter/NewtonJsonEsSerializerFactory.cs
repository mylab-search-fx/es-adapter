using System;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Creates NewtonJson serializer 
    /// </summary>
    public class NewtonJsonEsSerializerFactory : IEsSerializerFactory
    {
        private readonly JsonSerializer _jsonSerializer;

        /// <summary>
        /// Initializes a new instance of <see cref="NewtonJsonEsSerializerFactory"/>
        /// </summary>
        public NewtonJsonEsSerializerFactory()
            :this(new JsonSerializer())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NewtonJsonEsSerializerFactory"/>
        /// </summary>
        public NewtonJsonEsSerializerFactory(JsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        /// <inheritdoc />
        public IElasticsearchSerializer Create(IElasticsearchSerializer builtin, IConnectionSettingsValues settings)
        {
            return new NewtonJsonEsSerializer(_jsonSerializer);
        }
    }
}