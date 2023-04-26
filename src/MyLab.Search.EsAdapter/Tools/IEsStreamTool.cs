using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special stream
    /// </summary>
    public interface IEsStreamTool
    {
        /// <summary>
        /// Creates the stream
        /// </summary>
        /// <returns>Stream deleter</returns>
        Task<IAsyncDisposable> CreateAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the stream
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the stream exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }
}