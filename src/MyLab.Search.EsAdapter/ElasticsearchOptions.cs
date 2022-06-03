using MyLab.Search.EsAdapter.Serialization;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Contains options parameters for ES tools
    /// </summary>
    public class ElasticsearchOptions
    {
        /// <summary>
        /// ES address
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Index name which will be used when no index specified for model
        /// </summary>
        public string DefaultIndex { get; set; }

        /// <summary>
        /// User object serializer factory
        /// </summary>
        public IEsSerializerFactory SerializerFactory { get; set; }
    }
}