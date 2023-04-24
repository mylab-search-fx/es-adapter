using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with indexes
    /// </summary>
    public interface IEsIndexTools
    {
        /// <summary>
        /// Creates the index
        /// </summary>
        /// <returns>Index deleter</returns>
        Task<IAsyncDisposable> CreateIndexAsync(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates the index
        /// </summary>
        /// <returns>Index deleter</returns>
        Task<IAsyncDisposable> CreateIndexAsync(string indexName, string jsonSettings, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the index
        /// </summary>
        Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the index exists
        /// </summary>
        Task<bool> IsIndexExistentAsync(string indexName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Prune index
        /// </summary>
        Task PruneIndexAsync(string indexName, CancellationToken cancellationToken = default);
    }
}