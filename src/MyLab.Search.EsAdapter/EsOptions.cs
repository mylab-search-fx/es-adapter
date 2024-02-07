    using System;
    using System.Linq;
    using System.Reflection;
    using MyLab.Log;
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
        /// Index bindings
        /// </summary>
        public IndexBinding[] IndexBindings { get; set; }

        /// <summary>
        /// Serializer factory
        /// </summary>
        public IEsSerializerFactory SerializerFactory { get; set; }

        /// <summary>
        /// Determines whether the request and response body will be added to the dumps 
        /// </summary>
        public bool IncludeDumpBody { get; set; } = true;
    }

    /// <summary>
    /// Determine binding between document type and elasticsearch index
    /// </summary>
    public class IndexBinding
    {
        /// <summary>
        /// Document key from <see cref="EsBindingKeyAttribute"/> of type name
        /// </summary>
        public string Doc { get; set; }
        /// <summary>
        /// Index name
        /// </summary>
        public string Index { get; set; }
    }
}
