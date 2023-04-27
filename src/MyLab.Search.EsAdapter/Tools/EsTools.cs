using MyLab.Search.EsAdapter.Inter;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Default implementation for <see cref="IEsTools"/>
    /// </summary>
    public class EsTools : IEsTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <inheritdoc />
        public IEsSerializer Serializer { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsTools"/>
        /// </summary>
        public EsTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
            Serializer = new EsSerializer(clientProvider);
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
    }
}