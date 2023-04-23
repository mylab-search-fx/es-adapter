using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Default implementation for <see cref="IEsStreamTools"/>
    /// </summary>
    public class EsStreamTools : IEsStreamTools
    {
        private readonly IEsClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EsStreamTools"/>
        /// </summary>
        public EsStreamTools(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposable> CreateStreamAsync(string streamName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, $"_data_stream/{streamName}", cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the stream");

            return new StreamDeleter(streamName, this);
        }

        /// <inheritdoc />
        public async Task DeleteStreamAsync(string streamName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.DELETE, $"_data_stream/{streamName}", cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete the stream");
        }

        /// <inheritdoc />
        public async Task<bool> IsStreamExistsAsync(string streamName, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.GET, $"_data_stream/{streamName}", cancellationToken);

            EsException.ThrowIfInvalid(resp);

            return resp.HttpStatusCode != 404;
        }

        class StreamDeleter : IAsyncDisposable
        {
            private readonly string _indexName;
            private readonly IEsStreamTools _strmTools;

            public StreamDeleter(string indexName, IEsStreamTools strmTools)
            {
                _indexName = indexName;
                _strmTools = strmTools;
            }

            public async ValueTask DisposeAsync()
            {
                await _strmTools.DeleteStreamAsync(_indexName);
            }
        }
    }
}