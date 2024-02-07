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
    /// Provides abilities to work with indexes
    /// </summary>
    public interface IEsIndexesTool
    {
        /// <summary>
        /// Gets selected indexes
        /// </summary>
        Task<IEnumerable<FoundIndex>> GetAsync(
                Func<GetIndexDescriptor, IGetIndexRequest> selector = null, 
                CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Deleted selected indexes
        /// </summary>
        Task DeleteAsync(
                Func<DeleteIndexDescriptor, IDeleteIndexRequest> selector = null, 
                CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Creates new index with request object
        /// </summary>
        Task<IEsIndexTool> CreateAsync(
                ICreateIndexRequest createRequest,
                CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Creates new index with specified settings
        /// </summary>
        Task<IEsIndexTool> CreateAsync(
                string indexName,
                string jsonSettings,
                CancellationToken cancellationToken = default
            );
    }

    /// <summary>
    /// Represent a found index
    /// </summary>
    public record FoundIndex(string Name, IndexState State, IEsIndexTool Tool);

    class EsIndexesTool : IEsIndexesTool
    {
        private readonly IEsClientProvider _clientProvider;
        private readonly IEsResponseValidator _responseValidator;

        public EsIndexesTool(IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
            _responseValidator = responseValidator;
        }
        public async Task<IEnumerable<FoundIndex>> GetAsync(
                Func<GetIndexDescriptor, IGetIndexRequest> selector = null, 
                CancellationToken cancellationToken = default
            )
        {
            var response = await _clientProvider.Provide().Indices.GetAsync(Indices.All, selector, cancellationToken);
            
            _responseValidator.Validate(response, "Unable to get indexes");

            return response.Indices.Select(i =>
                new FoundIndex(i.Key.Name, i.Value, new EsIndexTool(i.Key.Name, _clientProvider, _responseValidator))
            );
        }

        public async Task DeleteAsync(
                Func<DeleteIndexDescriptor, IDeleteIndexRequest> selector = null, 
                CancellationToken cancellationToken = default
            )
        {
            var response = await _clientProvider.Provide().Indices.DeleteAsync(Indices.All, selector, cancellationToken);

            _responseValidator.Validate(response, "Unable to delete indexes");
        }

        public async Task<IEsIndexTool> CreateAsync(
                ICreateIndexRequest createRequest, 
                CancellationToken cancellationToken = default
            )
        {
            var resp = await _clientProvider.Provide().Indices
                .CreateAsync(createRequest.Index, _ => createRequest, cancellationToken);

            _responseValidator.Validate(resp, "Unable to create the index");

            return new EsIndexTool(createRequest.Index.Name, _clientProvider, _responseValidator);
        }

        public async Task<IEsIndexTool> CreateAsync(
                string indexName, 
                string jsonSettings, 
                CancellationToken cancellationToken = default
            )
        {
            var resp = await _clientProvider.Provide().LowLevel.Indices.CreateAsync<CreateIndexResponse>(
                indexName, jsonSettings, null, cancellationToken);

            _responseValidator.Validate(resp, "Unable to create the index");

            return new EsIndexTool(indexName, _clientProvider, _responseValidator);
        }
    }
}
