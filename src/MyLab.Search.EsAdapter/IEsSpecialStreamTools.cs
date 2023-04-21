using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Provides abilities to work with special stream
    /// </summary>
    public interface IEsSpecialStreamTools
    {
        /// <summary>
        /// Creates the stream
        /// </summary>
        Task<IStreamDeleter> CreateStreamAsync(CancellationToken cancellationToken = default);
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