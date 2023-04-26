using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special index template
    /// </summary>
    public interface IEsIndexTemplateTool
    {
        /// <summary>
        /// Creates or updates index template
        /// </summary>
        /// <returns>index template deleter</returns>
        Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates index template
        /// </summary>
        /// <returns>index template deleter</returns>
        Task<IAsyncDisposable> PutAsync(IPutIndexTemplateV2Request request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries get index template
        /// </summary>
        /// <returns>index template or null if exists or null</returns>
        public Task<IndexTemplate> TryGetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes index template
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets whether the index template exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }
}