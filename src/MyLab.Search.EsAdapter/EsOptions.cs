    using MyLab.Search.EsAdapter.Inter;

    namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Contains Elasticsearch client options
    /// </summary>
    public class EsOptions
    {
        /// <summary>
        /// Elasticsearch address
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Serializer factory
        /// </summary>
        public IEsSerializerFactory SerializerFactory { get; set; }
    }
}
