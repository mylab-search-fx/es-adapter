using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace MyLab.Search.EsAdapter.Search
{
    class EsSearcher<TDoc> : IEsSearcher<TDoc>
        where TDoc : class
    {
        private readonly IEsSearcher _baseSearcher;
        private readonly Lazy<string> _indexName;

        public EsSearcher(IEsSearcher baseSearcher, IOptions<EsOptions> options)
            :this(baseSearcher, options.Value)
        {
        }

        public EsSearcher(IEsSearcher baseSearcher, EsOptions options)
        {
            _baseSearcher = baseSearcher;
            _indexName = new Lazy<string>(options.GetIndexForDocType<TDoc>);
        }

        public Task<EsFound<TDoc>> SearchAsync(
            EsSearchParams<TDoc> searchParams, 
            CancellationToken cancellationToken = default) 
        {
            var indexName = _indexName.Value;
            return _baseSearcher.SearchAsync(indexName, searchParams, cancellationToken);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(
            EsSearchParams<TDoc> searchParams, 
            EsHlSelector<TDoc> highlight,
            CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;
            return _baseSearcher.SearchAsync(indexName, searchParams, highlight, cancellationToken);
        }
    }
}