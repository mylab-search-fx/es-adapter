using System;
using Elasticsearch.Net;

namespace IntegrationTests
{
    static class TestTools
    {
        public static readonly IConnectionPool ConnectionPool;

        static TestTools()
        {
            ConnectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }
    }
}