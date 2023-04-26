namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with Elasticsearch internals
    /// </summary>
    public interface IEsTools
    {
        /// <summary>
        /// Gets Elasticsearch component deserializer used es-client serializer
        /// </summary>
        IEsDeserializer Deserializer { get; }

        /// <summary>
        /// Creates special index tool
        /// </summary>
        IEsIndexTool Index(string indexName);

        /// <summary>
        /// Creates special stream tool
        /// </summary>
        IEsStreamTool Stream(string streamName);

        /// <summary>
        /// Creates special lifecycle policy tool
        /// </summary>
        IEsLifecyclePolicyTool LifecyclePolicy(string lifecyclePolicyId);

        /// <summary>
        /// Creates special component template tool
        /// </summary>
        IEsComponentTemplateTool ComponentTemplate(string componentTemplateName);

        /// <summary>
        /// Creates special index template tool
        /// </summary>
        IEsIndexTemplateTool IndexTemplate(string indexTemplateName);
    }
}
