using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special index 
    /// </summary>
    public interface IEsSpecialIndexTools
    {
        /// <summary>
        /// Creates the index
        /// </summary>
        /// <returns>Index deleter</returns>
        Task<IAsyncDisposable> CreateIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates the index
        /// </summary>
        /// <returns>Index deleter</returns>
        Task<IAsyncDisposable> CreateIndexAsync(string jsonSettings, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the index
        /// </summary>
        Task DeleteIndexAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the index exists
        /// </summary>
        Task<bool> IsIndexExistentAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Prune index
        /// </summary>
        Task PruneIndexAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides abilities to work with special index
    /// </summary>
    /// <remarks>Special for injection</remarks>
    public interface IEsSpecialIndexTools<TDoc> : IEsSpecialIndexTools where TDoc : class
    {
    }
}