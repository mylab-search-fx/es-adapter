using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides tools to manage special life cycle
    /// </summary>
    public interface IEsLifecyclePolicyTool
    {
        /// <summary>
        /// Creates or updates lifecycle
        /// </summary>
        /// <returns>Lifecycle deleter</returns>
        public Task<IAsyncDisposable> PutAsync(IPutLifecycleRequest lifecycleRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates lifecycle
        /// </summary>
        /// <returns>Lifecycle deleter</returns>
        public Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries get lifecycle policy
        /// </summary>
        /// <returns>lifecycle if exists or null</returns>
        public Task<LifecyclePolicy> TryGetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes lifecycle
        /// </summary>
        public Task DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets whether the lifecycle policy exists
        /// </summary>
        public Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }
    class EsLifecyclePolicyTool : IEsLifecyclePolicyTool
    {
        private readonly string _policyId;
        private readonly IEsClientProvider _clientProvider;
        private readonly IEsResponseValidator _responseValidator;

        public EsLifecyclePolicyTool(string policyId, IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _policyId = policyId;
            _clientProvider = clientProvider;
            _responseValidator = responseValidator;
        }

        public async Task<IAsyncDisposable> PutAsync(IPutLifecycleRequest lifecycleRequest, CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().IndexLifecycleManagement.PutLifecycleAsync(lifecycleRequest, cancellationToken);

            _responseValidator.Validate(resp, "Unable to put the lifecycle");

            return new LifecycleDeleter(this);
        }

        public async Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken)
        {
            if (jsonRequest == null) throw new ArgumentNullException(nameof(jsonRequest));

            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, "_ilm/policy/" + _policyId, cancellationToken,
                    jsonRequest);

            _responseValidator.Validate(resp, "Unable to put the lifecycle");

            return new LifecycleDeleter(this);
        }

        public async Task<LifecyclePolicy> TryGetAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().IndexLifecycleManagement.GetLifecycleAsync(d => d.PolicyId(_policyId), cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return null;

            _responseValidator.Validate(resp, "Unable to get lifecycle");

            return resp.Policies[_policyId];
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().IndexLifecycleManagement.DeleteLifecycleAsync(_policyId, d => d, cancellationToken);

            _responseValidator.Validate(resp, "Unable to delete lifecycle");
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            var resp = await _clientProvider.Provide().IndexLifecycleManagement
                .GetLifecycleAsync(s => s.PolicyId(_policyId), cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return false;

            _responseValidator.Validate(resp, "Unable to get lifecycle info");

            return true;
        }

        class LifecycleDeleter : IAsyncDisposable
        {
            private readonly IEsLifecyclePolicyTool _tool;

            public LifecycleDeleter(IEsLifecyclePolicyTool tool)
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