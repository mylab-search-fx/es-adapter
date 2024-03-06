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
        /// Gets tool to work with index collection
        /// </summary>
        IEsIndexesTool Indexes { get; }

        /// <summary>
        /// Gets tool to work with stream collection
        /// </summary>
        IEsStreamsTool Streams { get; }

        /// <summary>
        /// Gets tool to work with index template collection
        /// </summary>
        IEsIndexTemplatesTool IndexTemplates { get; }


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
        private readonly IEsResponseValidator _responseValidator;

        /// <inheritdoc />
        public IEsSerializer Serializer { get; }

        /// <inheritdoc />
        public IEsIndexesTool Indexes { get; }

        /// <inheritdoc />
        public IEsStreamsTool Streams { get; }

        /// <inheritdoc />
        public IEsIndexTemplatesTool IndexTemplates { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsTools"/>
        /// </summary>
        public EsTools(IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _clientProvider = clientProvider;
            _responseValidator = responseValidator;
            Serializer = new EsSerializer(clientProvider);
            Indexes = new EsIndexesTool(clientProvider, responseValidator);
            Streams = new EsStreamsTool(clientProvider, responseValidator);
            IndexTemplates = new EsIndexTemplatesTool(clientProvider, responseValidator);
        }

        /// <inheritdoc />
        public IEsIndexTool Index(string indexName)
        {
            return new EsIndexTool(indexName, _clientProvider, _responseValidator);
        }

        /// <inheritdoc />
        public IEsStreamTool Stream(string streamName)
        {
            return new EsStreamTool(streamName, _clientProvider, _responseValidator);
        }

        /// <inheritdoc />
        public IEsLifecyclePolicyTool LifecyclePolicy(string lifecyclePolicyId)
        {
            return new EsLifecyclePolicyTool(lifecyclePolicyId, _clientProvider, _responseValidator);
        }

        /// <inheritdoc />
        public IEsComponentTemplateTool ComponentTemplate(string componentTemplateName)
        {
            return new EsComponentTemplateTool(componentTemplateName, _clientProvider, _responseValidator);
        }

        /// <inheritdoc />
        public IEsIndexTemplateTool IndexTemplate(string indexTemplateName)
        {
            return new EsIndexTemplateTool(indexTemplateName, _clientProvider, _responseValidator);
        }

        /// <inheritdoc />
        public IEsAliasesTool Aliases()
        {
            return new EsAliasesTool(_clientProvider, _responseValidator);
        }
    }
}
