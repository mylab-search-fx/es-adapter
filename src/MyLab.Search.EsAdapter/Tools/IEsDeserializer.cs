using Nest;
using System.IO;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Deserializes Elasticsearch component used client serializer
    /// </summary>
    public interface IEsDeserializer
    {
        /// <summary>
        /// Deserializes a lifecycle policy
        /// </summary>
        LifecyclePolicy DeserializeLifecyclePolicy(Stream stream);

        /// <summary>
        /// Deserializes an index template
        /// </summary>
        IndexTemplate DeserializeIndexTemplate(Stream stream);

        /// <summary>
        /// Deserializes a component template
        /// </summary>
        ComponentTemplate DeserializeComponentTemplate(Stream stream);
    }
}
