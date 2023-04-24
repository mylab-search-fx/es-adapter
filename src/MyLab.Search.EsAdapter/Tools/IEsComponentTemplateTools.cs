using Nest;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with component templates
    /// </summary>
    public interface IEsComponentTemplateTools
    {
        /// <summary>
        /// Creates or updates component template
        /// </summary>
        /// <returns>component template deleter</returns>
        Task<IAsyncDisposable> PutComponentTemplateAsync(string templateName, string jsonRequest, CancellationToken cancellationToken);
        
        /// <summary>
        /// Creates or updates component template
        /// </summary>
        /// <returns>component template deleter</returns>
        Task<IAsyncDisposable> PutComponentTemplateAsync(IPutComponentTemplateRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Tries get component template
        /// </summary>
        /// <returns>component template or null if exists or null</returns>
        public Task<ComponentTemplate> TryGetComponentTemplateAsync(string templateName, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes component template
        /// </summary>
        Task DeleteComponentTemplateAsync(string templateName, CancellationToken cancellationToken);

        /// <summary>
        /// Gets whether the lifecycle policy exists
        /// </summary>
        Task<bool> IsComponentTemplateExistentAsync(string templateName, CancellationToken cancellationToken);
    }
}
