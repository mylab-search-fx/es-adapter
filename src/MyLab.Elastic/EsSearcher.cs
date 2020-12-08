using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    class EsSearcher<TDoc> : IEsSearcher<TDoc>
        where TDoc : class
    {
        private readonly ElasticClient _client;

        public EsSearcher(IEsClientProvider clientProvider)
        {
            _client = clientProvider.Provide();
        }

        public Task<EsFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams)
        {
            return new EsLogic<TDoc>(_client).SearchAsync(indexName, searchParams);
        }

        public Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams)
        {
            return SearchAsync(null, searchParams);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector)
        {
            return new EsLogic<TDoc>(_client).SearchAsync(indexName, searchParams, hlSelector);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector)
        {
            return SearchAsync(null, searchParams, hlSelector);
        }

        public IIndexSpecificEsSearcher<TDoc> ForIndex(string indexName)
        {
            return new IndexSpecificEsSearcher(indexName, _client);
        }

        class IndexSpecificEsSearcher : IIndexSpecificEsSearcher<TDoc>
        {
            private readonly ElasticClient _client;
            public string IndexName { get;}

            public IndexSpecificEsSearcher(string indexName, ElasticClient client)
            {
                IndexName = indexName;
                _client = client;
            }

            public Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams)
            {
                return new EsLogic<TDoc>(_client).SearchAsync(IndexName, searchParams);
            }

            public Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector)
            {
                return new EsLogic<TDoc>(_client).SearchAsync(IndexName, searchParams, hlSelector);
            }
        }
    }
}