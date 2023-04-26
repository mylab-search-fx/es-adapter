using System;
using System.Threading;
using System.Threading.Tasks;
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
}