using System;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;

namespace IntegrationTests
{
    static class TestTools
    {
        public static readonly IConnectionPool ConnectionPool;

        public static readonly IEsResponseValidator
            ResponseValidator = new EsResponseValidator(includeBodyInDump: true);

        static TestTools()
        {
            ConnectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        }
    }
}