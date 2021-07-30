using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Provides manager ES functions
    /// </summary>
    public interface IEsManager
    {
        Task<bool> PingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates index
        /// </summary>
        Task<IAsyncDisposable> CreateIndexAsync(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates default index
        /// </summary>
        Task<IAsyncDisposable> CreateDefaultIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete index
        /// </summary>
        Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);

        /// <summary>
        /// True if spcified index exists
        /// </summary>
        Task<bool> IsIndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
    }
}
