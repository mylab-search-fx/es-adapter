using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special stream
    /// </summary>
    public interface IEsSpecialStreamTools
    {
        /// <summary>
        /// Creates the stream
        /// </summary>
        /// <returns>Stream deleter</returns>
        Task<IAsyncDisposable> CreateStreamAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the stream
        /// </summary>
        Task DeleteStreamAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the stream exists
        /// </summary>
        Task<bool> IsStreamExistsAsync(CancellationToken cancellationToken = default);
    }
}