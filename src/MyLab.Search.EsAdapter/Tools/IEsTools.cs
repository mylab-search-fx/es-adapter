using MyLab.Search.EsAdapter.Inter;

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
        IEsSerializer Serializer { get; }

        /// <summary>
        /// Gets Indexes tool to work with index collection
        /// </summary>
        IEsIndexesTool Indexes { get; }

        /// <summary>
        /// Gets Streams tool to work with stream collection
        /// </summary>
        IEsStreamsTool Streams { get; }

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

        /// <summary>
        /// Creates aliases tool
        /// </summary>
        IEsAliasesTool Aliases();
    }

    /// <summary>
    /// Default implementation for <see cref="IEsTools"/>
    /// </summary>
    public class EsTools : IEsTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <inheritdoc />
        public IEsSerializer Serializer { get; }

        /// <inheritdoc />
        public IEsIndexesTool Indexes { get; }

        /// <inheritdoc />
        public IEsStreamsTool Streams { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsTools"/>
        /// </summary>
        public EsTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
            Serializer = new EsSerializer(clientProvider);
            Indexes = new EsIndexesTool(clientProvider);
            Streams = new EsStreamsTool(clientProvider);
        }

        /// <inheritdoc />
        public IEsIndexTool Index(string indexName)
        {
            return new EsIndexTool(indexName, _clientProvider);
        }

        /// <inheritdoc />
        public IEsStreamTool Stream(string streamName)
        {
            return new EsStreamTool(streamName, _clientProvider);
        }

        /// <inheritdoc />
        public IEsLifecyclePolicyTool LifecyclePolicy(string lifecyclePolicyId)
        {
            return new EsLifecyclePolicyTool(lifecyclePolicyId, _clientProvider);
        }

        /// <inheritdoc />
        public IEsComponentTemplateTool ComponentTemplate(string componentTemplateName)
        {
            return new EsComponentTemplateTool(componentTemplateName, _clientProvider);
        }

        /// <inheritdoc />
        public IEsIndexTemplateTool IndexTemplate(string indexTemplateName)
        {
            return new EsIndexTemplateTool(indexTemplateName, _clientProvider);
        }

        /// <inheritdoc />
        public IEsAliasesTool Aliases()
        {
            return new EsAliasesTool(_clientProvider);
        }
    }
}
