using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace MyLab.Search.EsAdapter.Search
{
    public class EsSearcher<TDoc> : IEsSearcher<TDoc>
        where TDoc : class
    {
        private readonly IEsSearcher _baseSearcher;
        private readonly string _indexName;

        public EsSearcher(IEsSearcher baseSearcher, IOptions<EsOptions> options)
            :this(baseSearcher, options.Value)
        {
        }

        public EsSearcher(IEsSearcher baseSearcher, EsOptions options)
            : this(baseSearcher, new OptionsIndexNameProvider(options))
        {
        }

        public EsSearcher(IEsSearcher baseSearcher, IIndexNameProvider indexNameProvider)
        {
            _baseSearcher = baseSearcher;
            _indexName = indexNameProvider.Provide<TDoc>();
        }

        public Task<EsFound<TDoc>> SearchAsync(
            EsSearchParams<TDoc> searchParams, 
            CancellationToken cancellationToken = default) 
        {
            return _baseSearcher.SearchAsync(_indexName, searchParams, cancellationToken);
        }

        public Task<EsHlFound<TDoc>> SearchAsync(
            EsSearchParams<TDoc> searchParams, 
            EsHlSelector<TDoc> highlight,
            CancellationToken cancellationToken = default)
        {
            return _baseSearcher.SearchAsync(_indexName, searchParams, highlight, cancellationToken);
        }
    }
}