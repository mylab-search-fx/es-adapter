using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides tools to manage life cycles
    /// </summary>
    public interface IEsLifecycleTools
    {
        /// <summary>
        /// Creates or updates lifecycle
        /// </summary>
        /// <returns>Lifecycle deleter</returns>
        public Task<IAsyncDisposable> PutLifecyclePolicyAsync(IPutLifecycleRequest lifecycleRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Creates or updates lifecycle
        /// </summary>
        /// <returns>Lifecycle deleter</returns>
        public Task<IAsyncDisposable> PutLifecyclePolicyAsync(string policyId, string jsonRequest, CancellationToken cancellationToken);
        
        /// <summary>
        /// Tries get lifecycle policy
        /// </summary>
        /// <returns>lifecycle if exists or null</returns>
        public Task<LifecyclePolicy> TryGetLifecyclePolicyAsync(string policyId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes lifecycle
        /// </summary>
        public Task DeleteLifecycleAsync(string policyId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets whether the lifecycle policy exists
        /// </summary>
        public Task<bool> IsLifecyclePolicyExistentAsync(string policyId, CancellationToken cancellationToken);
    }
}
