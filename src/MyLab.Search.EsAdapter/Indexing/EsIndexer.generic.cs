using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Search.EsAdapter.Indexing
{
    /// <summary>
    /// Default implementation for <see cref="IEsIndexer{TDoc}"/>
    /// </summary>
    public class EsIndexer<TDoc> : IEsIndexer<TDoc>
        where TDoc : class 
    {
        private readonly IEsIndexer _baseIndexer;
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexer"/>
        /// </summary>
        public EsIndexer(IEsIndexer baseIndexer, IOptions<EsOptions> options)
            : this(baseIndexer, options.Value)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexer"/>
        /// </summary>
        public EsIndexer(IEsIndexer baseIndexer, EsOptions options)
            : this(baseIndexer, new OptionsIndexNameProvider(options))
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexer"/>
        /// </summary>
        public EsIndexer(IEsIndexer baseIndexer, IIndexNameProvider indexNameProvider)
        {
            _baseIndexer = baseIndexer;
            _indexName = indexNameProvider.Provide<TDoc>();
        }
        /// <inheritdoc />
        public Task CreateAsync(TDoc doc, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.CreateAsync<TDoc>(_indexName, doc, cancellationToken);
        }
        /// <inheritdoc />
        public Task IndexAsync(TDoc doc, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.IndexAsync<TDoc>(_indexName, doc, cancellationToken);
        }
        /// <inheritdoc />
        public Task UpdateAsync(Id id, Expression<Func<TDoc>> factoryExpression, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.UpdateAsync<TDoc>(_indexName,id, factoryExpression, cancellationToken);
        }
        /// <inheritdoc />
        public Task UpdateAsync(TDoc partialDocument, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.UpdateAsync<TDoc>(_indexName, partialDocument, cancellationToken);
        }
        /// <inheritdoc />
        public Task DeleteAsync(Id docId, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.DeleteAsync(_indexName, docId, cancellationToken);
        }
        /// <inheritdoc />
        public Task BulkAsync(EsBulkIndexingRequest<TDoc> request, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.BulkAsync(_indexName, request, cancellationToken);
        }
    }
}