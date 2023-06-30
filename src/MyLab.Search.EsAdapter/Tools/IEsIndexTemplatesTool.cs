using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with index templates
    /// </summary>
    public interface IEsIndexTemplatesTool
    {
        /// <summary>
        /// Gets selected index templates
        /// </summary>
        Task<IEnumerable<FoundIndexTemplate>> GetAsync(
                Func<GetIndexTemplateV2Descriptor, IGetIndexTemplateV2Request> selector = null, 
                CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Deleted index templates with specified names
        /// </summary>
        Task DeleteByExactlyNamesAsync(string[] exactlyNames,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deleted index templates by name or name wildcard expression
        /// </summary>
        Task DeleteByNameOrWildcardExpressionAsync(string nameOrTemplate,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates new index template with request object
        /// </summary>
        Task<IEsIndexTemplateTool> PutAsync(
                IPutIndexTemplateV2Request putRequest,
                CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Creates new index template with string json request
        /// </summary>
        Task<IEsIndexTemplateTool> PutAsync(
            string name,
            string jsonRequest,
            CancellationToken cancellationToken = default
        );
    }

    /// <summary>
    /// Represent a found index template
    /// </summary>
    public record FoundIndexTemplate(string Name, IndexTemplate Template, IEsIndexTemplateTool Tool);

    class EsIndexTemplatesTool : IEsIndexTemplatesTool
    {
        private readonly IEsClientProvider _clientProvider;

        public EsIndexTemplatesTool(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        }
        public async Task<IEnumerable<FoundIndexTemplate>> GetAsync(
                Func<GetIndexTemplateV2Descriptor, IGetIndexTemplateV2Request> selector = null, 
                CancellationToken cancellationToken = default
            )
        {
            var response = await _clientProvider.Provide().Indices.GetTemplateV2Async(null,  selector, cancellationToken);
            
            EsException.ThrowIfInvalid(response, "Unable to get index templates");

            return response.IndexTemplates.Select(i =>
                new FoundIndexTemplate(i.Name, i.IndexTemplate, new EsIndexTemplateTool(i.Name, _clientProvider))
            );
        }

        public async Task DeleteByExactlyNamesAsync(string[] exactlyNames, 
                CancellationToken cancellationToken = default
            )
        {
            if (exactlyNames == null) throw new ArgumentNullException(nameof(exactlyNames));
            string oneName = string.Join(',', exactlyNames);
            var response = await _clientProvider.Provide().Indices.DeleteTemplateV2Async(oneName, _=>_, cancellationToken);

            EsException.ThrowIfInvalid(response, "Unable to delete index templates");
        }

        public async Task DeleteByNameOrWildcardExpressionAsync(string nameOrTemplate,
            CancellationToken cancellationToken = default
        )
        {
            if (nameOrTemplate == null) throw new ArgumentNullException(nameof(nameOrTemplate));
            var response = await _clientProvider.Provide().Indices.DeleteTemplateV2Async(nameOrTemplate, _ => _, cancellationToken);

            EsException.ThrowIfInvalid(response, "Unable to delete index templates");
        }

        public async Task<IEsIndexTemplateTool> PutAsync(
                IPutIndexTemplateV2Request putRequest, 
                CancellationToken cancellationToken = default
            )
        {
            var resp = await _clientProvider.Provide().Indices
                .PutTemplateV2Async(putRequest, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the index template");

            return new EsIndexTemplateTool(putRequest.Name.ToString(), _clientProvider);
        }

        public async Task<IEsIndexTemplateTool> PutAsync(
            string name,
            string jsonRequest,
            CancellationToken cancellationToken = default
        )
        {
            var tTool = new EsIndexTemplateTool(name, _clientProvider);
            await tTool.PutAsync(jsonRequest, cancellationToken);

            return tTool;
        }
    }
}
