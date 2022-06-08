using System;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class TestClientFixture
    {
        private readonly IConnectionPool _connectionPool;
        
        public TestClientFixture()
        {
            _connectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }

        public ElasticClient GetClientProvider(ITestOutputHelper output)
        {
            var settings = new ConnectionSettings(_connectionPool);

            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });

            return new ElasticClient(settings);
        }
    }
}