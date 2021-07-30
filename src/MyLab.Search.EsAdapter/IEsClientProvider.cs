using System;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Holds connection with ES
    /// </summary>
    public interface IEsClientProvider
    {
        /// <summary>
        /// Provides 
        /// </summary>
        /// <returns></returns>
        ElasticClient Provide();
    }

    class EsClientProvider : IEsClientProvider, IDisposable
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ElasticClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="IEsClientProvider"/>
        /// </summary>
        public EsClientProvider(ElasticsearchOptions options)
        {
            _connectionPool = new SingleNodeConnectionPool(new Uri(options.Url));
            var settings = new ConnectionSettings(_connectionPool);
            _client = new ElasticClient(settings);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IEsClientProvider"/>
        /// </summary>
        public EsClientProvider(IOptions<ElasticsearchOptions> options)
            : this(options.Value)
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

    public class SingleEsClientProvider : IEsClientProvider
    {
        private readonly ElasticClient _client;

        public SingleEsClientProvider(ElasticClient client)
        {
            _client = client;
        }
        public ElasticClient Provide()
        {
            return _client;
        }
    }
}
