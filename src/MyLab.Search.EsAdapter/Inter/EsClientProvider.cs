using System;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log.Dsl;
using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    class EsClientProvider : IEsClientProvider, IDisposable
    {
        private readonly EsOptions _options;
        private IConnectionPool _connectionPool;
        private readonly Lazy<ElasticClient> _client;
        private readonly IDslLogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="IEsClientProvider"/>
        /// </summary>
        public EsClientProvider(
            EsOptions options,
            ILogger<EsClientProvider> logger = null)
        {
            _options = options;
            _logger = logger?.Dsl();

            _client = new Lazy<ElasticClient>(CreateClient);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IEsClientProvider"/>
        /// </summary>
        public EsClientProvider(
            IOptions<EsOptions> options,
            ILogger<EsClientProvider> logger = null)
            : this(options.Value, logger)
        {

        }
        public ElasticClient Provide()
        {
            return _client.Value;
        }

        public void Dispose()
        {
            _connectionPool?.Dispose();
        }

        ElasticClient CreateClient()
        {
            if (_options.Url == null)
            {
                throw new InvalidOperationException("Elasticsearch URL is not specified");
            }

            _connectionPool = new SingleNodeConnectionPool(new Uri(_options.Url));

            var settings = _options.SerializerFactory == null
                ? new ConnectionSettings(_connectionPool)
                : new ConnectionSettings(_connectionPool, _options.SerializerFactory.Create);

            if (_logger != null)
            {
                settings.DisableDirectStreaming();
                settings.OnRequestCompleted(details =>
                {
                    _logger.Debug("ElasticSearch request completed")
                        .AndFactIs("dump", ApiCallDumper.ApiCallToDump(details))
                        .Write();
                });
            }

            return new ElasticClient(settings);
        }
    }
}