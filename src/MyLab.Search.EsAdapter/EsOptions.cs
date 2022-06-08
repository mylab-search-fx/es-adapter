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
        /// Gets bound index for document type 
        /// </summary>
        /// <exception cref="InvalidOperationException">Binding not found</exception>
        public string GetIndexForDocType<TDoc>()
        {
            IndexBinding found = null;

            var docType = typeof(TDoc);
            EsBindingKeyAttribute keyAttr = null;

            if (IndexBindings != null)
            {
                keyAttr = docType.GetCustomAttribute<EsBindingKeyAttribute>();

                found = IndexBindings.FirstOrDefault(
                    b => 
                        (keyAttr != null && b.Doc == keyAttr.Key) || 
                        b.Doc == docType.FullName ||
                        b.Doc == docType.Name);
            }

            if (found == null)
            {
                throw new InvalidOperationException("Document to index binding not found")
                    .AndFactIs("doc-type", docType.FullName)
                    .AndFactIs("binding-key", keyAttr != null ? keyAttr.Key : "[undefined]");
            }

            return found.Index;
        }
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
