using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special component template
    /// </summary>
    public interface IEsComponentTemplateTool
    {
        /// <summary>
        /// Creates or updates component template
        /// </summary>
        /// <returns>component template deleter</returns>
        Task<IAsyncDisposable> PutAsync(string jsonRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates component template
        /// </summary>
        /// <returns>component template deleter</returns>
        Task<IAsyncDisposable> PutAsync(IPutComponentTemplateRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries get component template
        /// </summary>
        /// <returns>component template or null if exists or null</returns>
        public Task<ComponentTemplate> TryGetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes component template
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets whether the component template exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }
}