using System;
using Elasticsearch.Net;

namespace IntegrationTests.Nest
{
    static class TestConnection
    {
        public static IConnectionPool Create()
        {
            var testEsAddr = Environment.GetEnvironmentVariable("TEST_ES_ADDR");

            if (string.IsNullOrEmpty(testEsAddr))
                throw new InvalidOperationException("TEST_ES_ADDR must be set");

            var uri = new Uri((!testEsAddr.StartsWith("http") ? "http://" : "") + testEsAddr);
            return new SingleNodeConnectionPool(uri);
        }
    }
}