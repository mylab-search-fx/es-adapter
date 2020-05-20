using System;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Elastic
{
    class DefaultEsManager : IEsManager, IDisposable
    {
        private readonly IConnectionPool _connectionPool;
        public ElasticClient Client { get; }

        public ElasticSearchOptions Options { get; }

        public DefaultEsManager(IOptions<ElasticSearchOptions> options)
            :this(options.Value)
        {
            
        }

        public DefaultEsManager(ElasticSearchOptions options)
        {
            Options = options;
            _connectionPool = new SingleNodeConnectionPool(new Uri(options.Url));
            var settings = new ConnectionSettings(_connectionPool);
            Client = new ElasticClient(settings);
        }

        public void Dispose()
        {
            _connectionPool.Dispose();
        }
    }
}