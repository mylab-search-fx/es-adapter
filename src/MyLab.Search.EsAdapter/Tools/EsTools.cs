using MyLab.Search.EsAdapter.Inter;

namespace MyLab.Search.EsAdapter.Tools
{
    class EsTools : IEsTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsTools"/>
        /// </summary>
        public EsTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public IEsIndexTool Index(string indexName)
        {
            return new EsIndexTool(indexName, _clientProvider);
        }

        public IEsStreamTool Stream(string streamName)
        {
            return new EsStreamTool(streamName, _clientProvider);
        }

        public IEsLifecyclePolicyTool LifecyclePolicy(string lifecyclePolicyId)
        {
            return new EsLifecyclePolicyTool(lifecyclePolicyId, _clientProvider);
        }

        public IEsComponentTemplateTool ComponentTemplate(string componentTemplateName)
        {
            return new EsComponentTemplateTool(componentTemplateName, _clientProvider);
        }

        public IEsComponentTemplateTool IndexTemplate(string indexTemplateName)
        {
            return new EsComponentTemplateTool(indexTemplateName, _clientProvider);
        }
    }
}