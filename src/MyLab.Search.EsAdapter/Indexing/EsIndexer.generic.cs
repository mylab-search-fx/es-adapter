using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Search.EsAdapter.Indexing
{
    public class EsIndexer<TDoc> : IEsIndexer<TDoc>
        where TDoc : class 
    {
        private readonly IEsIndexer _baseIndexer;
        private readonly string _indexName;

        public EsIndexer(IEsIndexer baseIndexer, IOptions<EsOptions> options)
            : this(baseIndexer, options.Value)
        {

        }

        public EsIndexer(IEsIndexer baseIndexer, EsOptions options)
            : this(baseIndexer, new OptionsIndexNameProvider(options))
        {

        }

        public EsIndexer(IEsIndexer baseIndexer, IIndexNameProvider indexNameProvider)
        {
            _baseIndexer = baseIndexer;
            _indexName = indexNameProvider.Provide<TDoc>();
        }

        public Task CreateAsync(TDoc doc, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.CreateAsync<TDoc>(_indexName, doc, cancellationToken);
        }

        public Task IndexAsync(TDoc doc, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.IndexAsync<TDoc>(_indexName, doc, cancellationToken);
        }

        public Task UpdateAsync(Id id, Expression<Func<TDoc>> factoryExpression, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.UpdateAsync<TDoc>(_indexName,id, factoryExpression, cancellationToken);
        }

        public Task UpdateAsync(TDoc partialDocument, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.UpdateAsync<TDoc>(_indexName, partialDocument, cancellationToken);
        }

        public Task DeleteAsync(Id docId, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.DeleteAsync(_indexName, docId, cancellationToken);
        }

        public Task BulkAsync(EsBulkIndexingRequest<TDoc> request, CancellationToken cancellationToken = default)
        {
            return _baseIndexer.BulkAsync(_indexName, request, cancellationToken);
        }
    }
}