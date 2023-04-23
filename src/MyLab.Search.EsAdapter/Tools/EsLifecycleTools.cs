using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Default implementation of <see cref="IEsLifecycleTools"/>
    /// </summary>
    public class EsLifecycleTools : IEsLifecycleTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsLifecycleTools"/>
        /// </summary>
        public EsLifecycleTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> PutLifecyclePolicyAsync(IPutLifecycleRequest lifecycleRequest)
        {
            await _clientProvider.Provide().IndexLifecycleManagement.PutLifecycleAsync(lifecycleRequest);

            return new LifecycleDeleter(lifecycleRequest.PolicyId.ToString(), this);
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> PutLifecyclePolicyAsync(string policyId, string jsonRequest, CancellationToken cancellationToken)
        {
            if (policyId == null) throw new ArgumentNullException(nameof(policyId));
            if (jsonRequest == null) throw new ArgumentNullException(nameof(jsonRequest));

            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, "_ilm/policy/" + policyId, cancellationToken,
                    jsonRequest);

            EsException.ThrowIfInvalid(resp, "Unable to delete the stream");

            return new LifecycleDeleter(policyId, this);
        }

        /// <inheritdoc />
        public Task DeleteLifecycleAsync(string policyId)
        {
            return _clientProvider.Provide().IndexLifecycleManagement.DeleteLifecycleAsync(policyId);
        }

        /// <inheritdoc />
        public async Task<bool> IsLifecyclePolicyExists(string policyId)
        {
            var resp = await _clientProvider.Provide().IndexLifecycleManagement.GetStatusAsync();

            EsException.ThrowIfInvalid(resp, "Unable to delete the stream");

            return resp.ApiCall.HttpStatusCode != 404;
        }

        class LifecycleDeleter : IAsyncDisposable
        {
            private readonly string _policyId;
            private readonly IEsLifecycleTools _tools;

            public LifecycleDeleter(string policyId, IEsLifecycleTools tools)
            {
                _policyId = policyId;
                _tools = tools;
            }
            public async ValueTask DisposeAsync()
            {
                await _tools.DeleteLifecycleAsync(_policyId);
            }
        }
    }
}