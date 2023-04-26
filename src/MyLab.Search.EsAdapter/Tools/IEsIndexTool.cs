using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special index
    /// </summary>
    public interface IEsIndexTool
    {
        /// <summary>
        /// Creates new index with <see cref="ICreateIndexRequest"/>
        /// </summary>
        Task<IAsyncDisposable> CreateAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates new index with json request
        /// </summary>
        Task<IAsyncDisposable> CreateAsync(string jsonSettings, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the index
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the index exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Prune index
        /// </summary>
        Task PruneAsync(CancellationToken cancellationToken = default);
    }
    
}