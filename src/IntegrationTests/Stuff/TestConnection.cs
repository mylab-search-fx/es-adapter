using System;
using Elasticsearch.Net;

namespace IntegrationTests.Stuff
{
    static class TestConnection
    {
        public static IConnectionPool Create()
        {
            return new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }
    }
}