using System;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    class TestEsClientProvider : IEsClientProvider, IDisposable
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ElasticClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="IEsClientProvider"/>
        /// </summary>
        public TestEsClientProvider(string url, ITestOutputHelper output)
        {
            _connectionPool = new SingleNodeConnectionPool(new Uri(url));
            var settings = new ConnectionSettings(_connectionPool);

            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });

            _client = new ElasticClient(settings);
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
