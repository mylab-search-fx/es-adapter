using System;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Provides manager ES functions
    /// </summary>
    public interface IEsManager
    {
        Task<bool> PingAsync();

        /// <summary>
        /// Creates index
        /// </summary>
        Task<IAsyncDisposable> CreateIndexAsync(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null);

        /// <summary>
        /// Creates default index
        /// </summary>
        Task<IAsyncDisposable> CreateDefaultIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null);

        /// <summary>
        /// Delete index
        /// </summary>
        Task DeleteIndexAsync(string indexName);

        /// <summary>
        /// True if spcified index exists
        /// </summary>
        Task<bool> IsIndexExistsAsync(string indexName);
    }
}
