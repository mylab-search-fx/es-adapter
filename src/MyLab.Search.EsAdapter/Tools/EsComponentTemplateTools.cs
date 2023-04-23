using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Default implementation of <see cref="IEsComponentTemplateTools"/>
    /// </summary>
    public class EsComponentTemplateTools : IEsComponentTemplateTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsComponentTemplateTools"/>
        /// </summary>
        public EsComponentTemplateTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> PutComponentTemplateAsync(string cTemplateId, string jsonRequest, CancellationToken cancellationToken)
        {
            if (cTemplateId == null) throw new ArgumentNullException(nameof(cTemplateId));
            if (jsonRequest == null) throw new ArgumentNullException(nameof(jsonRequest));
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, "_component_template/" + cTemplateId, cancellationToken,
                    jsonRequest);

            EsException.ThrowIfInvalid(resp, "Unable to put the component template");

            return new ComponentTemplateDeleter(cTemplateId, this);
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> PutComponentTemplateAsync(IPutComponentTemplateRequest request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            await _clientProvider.Provide().Cluster.PutComponentTemplateAsync(request, cancellationToken);

            return new ComponentTemplateDeleter(request.Name.ToString(), this);
        }

        /// <inheritdoc />
        public async Task DeleteComponentTemplateAsync(string cTemplateId)
        {
            var resp = await _clientProvider.Provide().Cluster.DeleteComponentTemplateAsync(cTemplateId);

            EsException.ThrowIfInvalid(resp, "Unable to delete the component template");
        }

        /// <inheritdoc />
        public async Task<bool> IsComponentTemplateExistsAsync(string cTemplateId)
        {
            var resp = await _clientProvider.Provide().Cluster.ComponentTemplateExistsAsync(cTemplateId);

            if (resp.ApiCall.HttpStatusCode == 404)
                return false;

            EsException.ThrowIfInvalid(resp, "Unable to detect the component template");

            return true;
        }

        class ComponentTemplateDeleter : IAsyncDisposable
        {
            private readonly string _cTemplateId;
            private readonly IEsComponentTemplateTools _tools;

            public ComponentTemplateDeleter(string cTemplateId, IEsComponentTemplateTools tools)
            {
                _cTemplateId = cTemplateId;
                _tools = tools;
            }
            public async ValueTask DisposeAsync()
            {
                await _tools.DeleteComponentTemplateAsync(_cTemplateId);
            }
        }
    }
}