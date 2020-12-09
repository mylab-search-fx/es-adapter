using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    public class EsSearcher<TDoc> : IEsSearcher<TDoc>
        where TDoc : class
    {
        private readonly IIndexNameProvider _indexNameProvider;
        private readonly EsLogic<TDoc> _logic;

        public EsSearcher(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider)
        {
            _indexNameProvider = indexNameProvider;
            var client = clientProvider.Provide();
            _logic = new EsLogic<TDoc>(client);
        }

        public Task<EsFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams)
        {
            return _logic.SearchAsync(indexName, searchParams);
        }

        public Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams)
        {
            return SearchAsync(_indexNameProvider.Provide<TDoc>(), searchParams);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector)
        {
            return _logic.SearchAsync(indexName, searchParams, hlSelector);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector)
        {
            return SearchAsync(_indexNameProvider.Provide<TDoc>(), searchParams, hlSelector);
        }

        public IIndexSpecificEsSearcher<TDoc> ForIndex(string indexName)
        {
            return new IndexSpecificEsSearcher(indexName, _logic);
        }

        class IndexSpecificEsSearcher : IIndexSpecificEsSearcher<TDoc>
        {
            private readonly EsLogic<TDoc> _logic;
            public string IndexName { get;}

            public IndexSpecificEsSearcher(string indexName, EsLogic<TDoc> logic)
            {
                _logic = logic;
                IndexName = indexName;
            }

            public Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams)
            {
                return _logic.SearchAsync(IndexName, searchParams);
            }

            public Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector)
            {
                return _logic.SearchAsync(IndexName, searchParams, hlSelector);
            }
        }
    }
}