using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Default implementation for <see cref="IEsSearcher{T}"/>
    /// </summary>
    /// <typeparam name="TDoc"></typeparam>
    public class EsSearcher<TDoc> : IEsSearcher<TDoc>
        where TDoc : class
    {
        private readonly IEsSearcher _baseSearcher;
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearcher{T}"/>
        /// </summary>
        public EsSearcher(IEsSearcher baseSearcher, IOptions<EsOptions> options)
            :this(baseSearcher, options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearcher{T}"/>
        /// </summary>
        public EsSearcher(IEsSearcher baseSearcher, EsOptions options)
            : this(baseSearcher, new OptionsIndexNameProvider(options))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearcher{T}"/>
        /// </summary>
        public EsSearcher(IEsSearcher baseSearcher, IIndexNameProvider indexNameProvider)
        {
            _baseSearcher = baseSearcher;
            _indexName = indexNameProvider.Provide<TDoc>();
        }

        /// <inheritdoc />
        public Task<EsFound<TDoc>> SearchAsync(
            EsSearchParams<TDoc> searchParams, 
            CancellationToken cancellationToken = default) 
        {
            return _baseSearcher.SearchAsync(_indexName, searchParams, cancellationToken);
        }

        /// <inheritdoc />
        public Task<EsHlFound<TDoc>> SearchAsync(
            EsSearchParams<TDoc> searchParams, 
            EsHlSelector<TDoc> highlight,
            CancellationToken cancellationToken = default)
        {
            return _baseSearcher.SearchAsync(_indexName, searchParams, highlight, cancellationToken);
        }
    }
}