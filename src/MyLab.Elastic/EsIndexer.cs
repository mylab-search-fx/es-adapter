using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{

    class EsIndexer<TDoc> : IEsIndexer<TDoc> 
        where TDoc : class
    {
        private readonly IIndexNameProvider _indexNameProvider;
        private readonly ElasticClient _client;

        public EsIndexer(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider)
        {
            _indexNameProvider = indexNameProvider;
            _client = clientProvider.Provide();
        }

        public Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents)
        {
            return new EsLogic<TDoc>(_client).IndexManyAsync(indexName, documents);
        }

        public Task IndexManyAsync(IEnumerable<TDoc> documents)
        {
            return IndexManyAsync(_indexNameProvider.Provide<TDoc>(), documents);
        }

        public Task IndexAsync(string indexName, TDoc document)
        {
            return new EsLogic<TDoc>(_client).IndexAsync(indexName, document);
        }

        public Task IndexAsync(TDoc document)
        {
            return IndexAsync(_indexNameProvider.Provide<TDoc>(), document);
        }

        public IIndexSpecificEsIndexer<TDoc> ForIndex(string indexName)
        {
            return new indexSpecificIndexer(indexName, _client);
        }

        class indexSpecificIndexer : IIndexSpecificEsIndexer<TDoc>
        {
            private readonly ElasticClient _client;
            public string IndexName { get; set; }

            public indexSpecificIndexer(string indexName, ElasticClient client)
            {
                IndexName = indexName;
                _client = client;
            }
            public Task IndexManyAsync(IEnumerable<TDoc> documents)
            {
                return new EsLogic<TDoc>(_client).IndexManyAsync(IndexName, documents);
            }

            public Task IndexAsync(TDoc document)
            {
                return new EsLogic<TDoc>(_client).IndexAsync(IndexName, document);
            }
        }
    }
}