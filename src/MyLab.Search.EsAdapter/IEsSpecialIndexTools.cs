using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Provides abilities to work with special index 
    /// </summary>
    public interface IEsSpecialIndexTools
    {
        /// <summary>
        /// Creates the index
        /// </summary>
        Task<IIndexDeleter> CreateIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates the index
        /// </summary>
        Task<IIndexDeleter> CreateIndexAsync(string jsonSettings, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the index
        /// </summary>
        Task DeleteIndexAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the index exists
        /// </summary>
        Task<bool> IsIndexExistsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Prune index
        /// </summary>
        Task PruneIndexAsync(CancellationToken cancellationToken = default);
    }
}