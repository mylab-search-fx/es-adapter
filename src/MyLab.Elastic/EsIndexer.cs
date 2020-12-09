using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{

    public class EsIndexer<TDoc> : IEsIndexer<TDoc> 
        where TDoc : class
    {
        private readonly IIndexNameProvider _indexNameProvider;
        private readonly EsLogic<TDoc> _logic;

        public EsIndexer(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider)
        {
            _indexNameProvider = indexNameProvider;
             var client = clientProvider.Provide();
            _logic= new EsLogic<TDoc>(client);
        }

        public Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents)
        {
            return _logic.IndexManyAsync(indexName, documents);
        }

        public Task IndexManyAsync(IEnumerable<TDoc> documents)
        {
            return IndexManyAsync(_indexNameProvider.Provide<TDoc>(), documents);
        }

        public Task IndexAsync(string indexName, TDoc document)
        {
            return _logic.IndexAsync(indexName, document);
        }

        public Task IndexAsync(TDoc document)
        {
            return IndexAsync(_indexNameProvider.Provide<TDoc>(), document);
        }

        public IIndexSpecificEsIndexer<TDoc> ForIndex(string indexName)
        {
            return new IndexSpecificIndexer(indexName, _logic);
        }

        public Task UpdateAsync(string indexName, string docId, Expression<Func<TDoc>> updateExpression,
            CancellationToken cancellationToken = default)
        {
            return _logic.UpdateAsync(indexName, docId, updateExpression, cancellationToken);
        }

        public Task UpdateAsync(string indexName, long docId, Expression<Func<TDoc>> updateExpression,
            CancellationToken cancellationToken = default)
        {
            return _logic.UpdateAsync(indexName, docId, updateExpression, cancellationToken);
        }

        class IndexSpecificIndexer : IIndexSpecificEsIndexer<TDoc>
        {
            private readonly EsLogic<TDoc> _logic;
            public string IndexName { get; set; }

            public IndexSpecificIndexer(string indexName, EsLogic<TDoc> logic)
            {
                _logic = logic;
                IndexName = indexName;
            }
            public Task IndexManyAsync(IEnumerable<TDoc> documents)
            {
                return _logic.IndexManyAsync(IndexName, documents);
            }

            public Task IndexAsync(TDoc document)
            {
                return _logic.IndexAsync(IndexName, document);
            }

            public Task UpdateAsync(string docId, Expression<Func<TDoc>> updateExpression,
                CancellationToken cancellationToken = default)
            {
                return _logic.UpdateAsync(IndexName, docId, updateExpression, cancellationToken);
            }

            public Task UpdateAsync(long docId, Expression<Func<TDoc>> updateExpression,
                CancellationToken cancellationToken = default)
            {
                return _logic.UpdateAsync(IndexName, docId, updateExpression, cancellationToken);
            }
        }
    }
}