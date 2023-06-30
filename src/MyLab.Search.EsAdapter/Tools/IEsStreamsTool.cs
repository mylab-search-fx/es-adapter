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
    /// Provides abilities to work with streams
    /// </summary>
    public interface IEsStreamsTool
    {
        /// <summary>
        /// Gets streams with selector
        /// </summary>
        Task<IEnumerable<FoundStream>> GetAsync(
            Func<GetDataStreamDescriptor, IGetDataStreamRequest> selector = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Delete selected streams
        /// </summary>
        Task DeleteAsync(
            Func<DeleteDataStreamDescriptor, IDeleteDataStreamRequest> selector = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Create stream with <see cref="ICreateDataStreamRequest"/>
        /// </summary>
        Task<IEsStreamTool> CreateAsync(
            ICreateDataStreamRequest createRequest,
            CancellationToken cancellationToken = default
        );
    }

    /// <summary>
    /// Represent a found stream
    /// </summary>
    public record FoundStream(string Name, DataStreamInfo Info, IEsStreamTool Tool);

    class EsStreamsTool : IEsStreamsTool
    {
        private readonly IEsClientProvider _clientProvider;

        public EsStreamsTool(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        }
        public async Task<IEnumerable<FoundStream>> GetAsync(
                Func<GetDataStreamDescriptor, IGetDataStreamRequest> selector = null, 
                CancellationToken cancellationToken = default
            )
        {
            var response = await _clientProvider.Provide().Indices
                .GetDataStreamAsync(null, selector, cancellationToken);
            
            EsException.ThrowIfInvalid(response, "Unable to get streams");

            return response.DataStreams.Select(i =>
                new FoundStream(i.Name, i, new EsStreamTool(i.Name, _clientProvider))
            );
        }

        public async Task DeleteAsync(
                Func<DeleteDataStreamDescriptor, IDeleteDataStreamRequest> selector = null, 
                CancellationToken cancellationToken = default
            )
        {
            var response = await _clientProvider.Provide().Indices.DeleteDataStreamAsync("*", selector, cancellationToken);

            EsException.ThrowIfInvalid(response, "Unable to delete streams");
        }

        public async Task<IEsStreamTool> CreateAsync(
                ICreateDataStreamRequest createRequest, 
                CancellationToken cancellationToken = default
            )
        {
            var resp = await _clientProvider.Provide().Indices
                .CreateDataStreamAsync(createRequest, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the stream");

            return new EsStreamTool(createRequest.Name.ToString(), _clientProvider);
        }
    }
}
