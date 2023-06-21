using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with special stream
    /// </summary>
    public interface IEsStreamTool
    {
        /// <summary>
        /// Creates the stream
        /// </summary>
        /// <returns>Stream deleter</returns>
        Task<IAsyncDisposable> CreateAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the stream
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets whether the stream exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes alias from stream
        /// </summary>
        Task DeleteAlias(string aliasName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Provides tool sof specified stream alias
        /// </summary>
        IEsAliasTool Alias(string aliasName);
    }

    class EsStreamTool : IEsStreamTool
    {
        private readonly string _streamName;
        private readonly IEsClientProvider _clientProvider;

        public EsStreamTool(string streamName, IEsClientProvider clientProvider)
        {
            _streamName = streamName ?? throw new ArgumentNullException(nameof(streamName));
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        }

        public async Task<IAsyncDisposable> CreateAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.PUT, $"_data_stream/{_streamName}", cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create the stream");

            return new StreamDeleter(this);
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.DELETE, $"_data_stream/{_streamName}", cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete the stream");
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.GET, $"_data_stream/{_streamName}", cancellationToken);

            if (resp.HttpStatusCode == 404) return false;

            EsException.ThrowIfInvalid(resp);

            return true;
        }

        public async Task DeleteAlias(string aliasName, CancellationToken cancellationToken = default)
        {
            if (aliasName == null) throw new ArgumentNullException(nameof(aliasName));

            var resp = await _clientProvider.Provide().LowLevel
                .DoRequestAsync<StringResponse>(HttpMethod.DELETE, $"{_streamName}/_alias/{aliasName}", cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete the alias");
        }

        public IEsAliasTool Alias(string aliasName)
        {
            if (aliasName == null) throw new ArgumentNullException(nameof(aliasName));
            return new EsAliasTool(aliasName, _streamName, _clientProvider);
        }

        class StreamDeleter : IAsyncDisposable
        {
            private readonly IEsStreamTool _strmTool;

            public StreamDeleter(IEsStreamTool strmTool)
            {
                _strmTool = strmTool;
            }

            public async ValueTask DisposeAsync()
            {
                await _strmTool.DeleteAsync();
            }
        }
    }
}