using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special component template
    /// </summary>
    public interface IEsComponentTemplateTool
    {
        /// <summary>
        /// Creates or updates component template
        /// </summary>
        /// <returns>component template deleter</returns>
        Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates component template
        /// </summary>
        /// <returns>component template deleter</returns>
        Task<IAsyncDisposable> PutAsync(IPutComponentTemplateRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries get component template
        /// </summary>
        /// <returns>component template or null if exists or null</returns>
        public Task<ComponentTemplate> TryGetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes component template
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets whether the component template exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }

    class EsComponentTemplateTool : IEsComponentTemplateTool
    {
        private readonly string _templateName;
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsComponentTemplateTool"/>
        /// </summary>
        public EsComponentTemplateTool(string templateName, IEsClientProvider clientProvider)
        {
            _templateName = templateName;
            _clientProvider = clientProvider;
        }

        public async Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken)
        {
            if (jsonRequest == null) throw new ArgumentNullException(nameof(jsonRequest));
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, "_component_template/" + _templateName, cancellationToken,
                    jsonRequest);

            EsException.ThrowIfInvalid(resp, "Unable to put component template");

            return new ComponentTemplateDeleter(this);
        }

        public async Task<IAsyncDisposable> PutAsync(IPutComponentTemplateRequest request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var resp = await _clientProvider.Provide().Cluster.PutComponentTemplateAsync(request, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to put component template");

            return new ComponentTemplateDeleter(this);
        }

        public async Task<ComponentTemplate> TryGetAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().Cluster.GetComponentTemplateAsync(_templateName, d => d, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return null;

            EsException.ThrowIfInvalid(resp, "Unable to get component template");

            return resp.ComponentTemplates.FirstOrDefault(t => t.Name == _templateName)?.ComponentTemplate;
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().Cluster.DeleteComponentTemplateAsync(_templateName, d => d, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete component template");
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().Cluster.ComponentTemplateExistsAsync(_templateName, d => d, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return false;

            EsException.ThrowIfInvalid(resp, "Unable to detect component template");

            return true;
        }

        class ComponentTemplateDeleter : IAsyncDisposable
        {
            private readonly IEsComponentTemplateTool _tool;

            public ComponentTemplateDeleter(IEsComponentTemplateTool tool)
            {
                _tool = tool;
            }
            public async ValueTask DisposeAsync()
            {
                await _tool.DeleteAsync(CancellationToken.None);
            }
        }
    }
}