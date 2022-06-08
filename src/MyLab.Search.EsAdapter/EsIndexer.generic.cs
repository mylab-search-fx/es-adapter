using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter
{
    class EsIndexer<TDoc> : IEsIndexer<TDoc>
        where TDoc : class 
    {
        private readonly IEsIndexer _baseIndexer;
        private readonly Lazy<string> _indexName;

        public EsIndexer(IEsIndexer baseIndexer, IOptions<EsOptions> options)
            : this(baseIndexer, options.Value)
        {

        }

        public EsIndexer(IEsIndexer baseIndexer, EsOptions options)
        {
            _baseIndexer = baseIndexer;
            _indexName = new Lazy<string>(options.GetIndexForDocType<TDoc>);
        }

        public Task CreateAsync(TDoc doc, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexer.CreateAsync<TDoc>(indexName, doc, cancellationToken);
        }

        public Task IndexAsync(TDoc doc, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexer.IndexAsync<TDoc>(indexName, doc, cancellationToken);
        }

        public Task UpdateAsync(Id id, Expression<Func<TDoc>> factoryExpression, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexer.UpdateAsync<TDoc>(indexName,id, factoryExpression, cancellationToken);
        }

        public Task UpdateAsync(TDoc partialDocument, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexer.UpdateAsync<TDoc>(indexName, partialDocument, cancellationToken);
        }

        public Task DeleteAsync(Id docId, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexer.DeleteAsync(indexName, docId, cancellationToken);
        }
    }
}