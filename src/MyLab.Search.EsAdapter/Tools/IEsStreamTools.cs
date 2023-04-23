using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with streams
    /// </summary>
    public interface IEsStreamTools
    {
        /// <summary>
        /// Creates the stream
        /// </summary>
        /// <returns>Stream deleter</returns>
        Task<IAsyncDisposable> CreateStreamAsync(string streamName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the stream
        /// </summary>
        Task DeleteStreamAsync(string streamName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the stream exists
        /// </summary>
        Task<bool> IsStreamExistsAsync(string streamName, CancellationToken cancellationToken = default);
    }
}