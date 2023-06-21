using Nest;
using System.IO;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using System;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Deserializes Elasticsearch component used client serializer
    /// </summary>
    public interface IEsSerializer
    {
        /// <summary>
        /// Deserializes an object
        /// </summary>
        T Deserialize<T>(Stream stream);
        
        /// <summary>
        /// Serializes an object
        /// </summary>
        void Serialize(object obj, Stream stream, SerializationFormatting serializationFormatting = SerializationFormatting.None);
    }

    class EsSerializer : IEsSerializer
    {
        private readonly ElasticClient _client;

        public EsSerializer(IEsClientProvider clientProvider)
        {
            if (clientProvider == null) throw new ArgumentNullException(nameof(clientProvider));

            _client = clientProvider.Provide();
        }

        public T Deserialize<T>(Stream stream)
        {
            return _client.SourceSerializer.Deserialize<T>(stream);
        }

        public void Serialize(object obj, Stream stream, SerializationFormatting serializationFormatting = SerializationFormatting.None)
        {
            _client.SourceSerializer.Serialize(obj, stream, serializationFormatting);
        }
    }
}
