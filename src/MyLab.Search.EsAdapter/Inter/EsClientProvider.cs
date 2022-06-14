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
        private readonly IConnectionPool _connectionPool;
        private readonly ElasticClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="IEsClientProvider"/>
        /// </summary>
        public EsClientProvider(
            EsOptions options,
            ILogger<EsClientProvider> logger = null)
        {
            if (options.Url == null)
            {
                throw new InvalidOperationException("Elasticsearch URL is not specified");
            }

            _connectionPool = new SingleNodeConnectionPool(new Uri(options.Url));

            var settings = options.SerializerFactory == null
                ? new ConnectionSettings(_connectionPool)
                : new ConnectionSettings(_connectionPool, options.SerializerFactory.Create);

            if (logger != null)
            {
                var log = logger.Dsl();

                settings.DisableDirectStreaming();
                settings.OnRequestCompleted(details =>
                {
                    log.Debug("ElasticSearch request completed")
                        .AndFactIs("dump", ApiCallDumper.ApiCallToDump(details))
                        .Write();
                });
            }

            _client = new ElasticClient(settings);
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
            return _client;
        }

        public void Dispose()
        {
            _connectionPool?.Dispose();
        }
    }
}