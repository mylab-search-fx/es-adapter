using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Provides methods to work with ElasticSearch
    /// </summary>

    public interface IEsManager
    {
        Task IndexAsync<TDoc>(TDoc document)
            where TDoc : class;

        Task<TDoc> SearchAsync<TDoc>(
            Func<SearchDescriptor<TDoc>, ISearchRequest> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector = null)
            where TDoc : class;
    }

    class DefaultEsManager : IEsManager, IDisposable
    {
        private readonly IConnectionPool _connectionPool;
        private ElasticClient _client;

        public DefaultEsManager(IOptions<ElasticSearchOptions> options)
            :this(options.Value)
        {
            
        }

        public DefaultEsManager(ElasticSearchOptions options)
            :this(new SingleNodeConnectionPool(new Uri(options.Url)))
        {

        }

        public DefaultEsManager(IConnectionPool connectionPool)
        {
            _connectionPool = connectionPool;
            var settings = new ConnectionSettings(_connectionPool);
            _client = new ElasticClient(settings);
        }

        public void Dispose()
        {
            _connectionPool.Dispose();
        }

        public Task IndexAsync<TDoc>(TDoc document) where TDoc : class
        {
            return Task.CompletedTask;
        }

        public Task<TDoc> SearchAsync<TDoc>(Func<SearchDescriptor<TDoc>, ISearchRequest> query, Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector = null) where TDoc : class
        {
            return Task.FromResult(default(TDoc));
        }
    }
}
