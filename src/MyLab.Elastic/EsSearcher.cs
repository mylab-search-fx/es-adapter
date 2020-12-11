using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Elastic
{
    public class EsSearcher<TDoc> : IEsSearcher<TDoc>
        where TDoc : class
    {
        private readonly IIndexNameProvider _indexNameProvider;
        private readonly EsLogic<TDoc> _logic;
        private readonly string _defaultIndexName;

        public EsSearcher(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider,
            IOptions<ElasticsearchOptions> options)
        : this(clientProvider, indexNameProvider, options.Value)
        {

        }
        public EsSearcher(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider, ElasticsearchOptions options)
        {
            _indexNameProvider = indexNameProvider;
            var client = clientProvider.Provide();
            _logic = new EsLogic<TDoc>(client);
            _defaultIndexName = options.DefaultIndex;
        }

        public Task<EsFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams, CancellationToken cancellationToken = default)
        {
            return _logic.SearchAsync(indexName, searchParams, cancellationToken);
        }

        public Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, CancellationToken cancellationToken = default)
        {
            return SearchAsync(_indexNameProvider.Provide<TDoc>(), searchParams, cancellationToken);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector, CancellationToken cancellationToken = default)
        {
            return _logic.SearchAsync(indexName, searchParams, hlSelector, cancellationToken);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector, CancellationToken cancellationToken = default)
        {
            return SearchAsync(_indexNameProvider.Provide<TDoc>(), searchParams, hlSelector, cancellationToken);
        }

        public IIndexSpecificEsSearcher<TDoc> ForIndex(string indexName)
        {
            return new IndexSpecificEsSearcher(indexName, _logic);
        }

        public IIndexSpecificEsSearcher<TDoc> ForDefaultIndex()
        {
            return ForIndex(_defaultIndexName);
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

            public Task<EsFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, CancellationToken cancellationToken = default)
            {
                return _logic.SearchAsync(IndexName, searchParams, cancellationToken);
            }

            public Task<EsHlFound<TDoc>> SearchAsync(SearchParams<TDoc> searchParams, EsHlSelector<TDoc> hlSelector, CancellationToken cancellationToken = default)
            {
                return _logic.SearchAsync(IndexName, searchParams, hlSelector, cancellationToken);
            }
        }
    }
}