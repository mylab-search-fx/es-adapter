using Nest;
using System.IO;
using Elasticsearch.Net;

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
}
