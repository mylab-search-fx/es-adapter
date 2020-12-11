using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;

namespace MyLab.Elastic
{

    public class EsIndexer<TDoc> : IEsIndexer<TDoc> 
        where TDoc : class
    {
        private readonly IIndexNameProvider _indexNameProvider;
        private readonly EsLogic<TDoc> _logic;
        private readonly string _defaultIndexName;

        public EsIndexer(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider,
            ElasticsearchOptions options)
        {
            _indexNameProvider = indexNameProvider;
            var client = clientProvider.Provide();
            _logic = new EsLogic<TDoc>(client);
            _defaultIndexName = options.DefaultIndex;
        }
        public EsIndexer(IEsClientProvider clientProvider, IIndexNameProvider indexNameProvider, IOptions<ElasticsearchOptions> options)
            : this(clientProvider, indexNameProvider, options.Value)
        {
            
        }

        public Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents, CancellationToken cancellationToken = default)
        {
            return _logic.IndexManyAsync(indexName, documents, cancellationToken);
        }

        public Task IndexManyAsync(IEnumerable<TDoc> documents, CancellationToken cancellationToken = default)
        {
            return IndexManyAsync(_indexNameProvider.Provide<TDoc>(), documents, cancellationToken);
        }

        public Task IndexAsync(string indexName, TDoc document, CancellationToken cancellationToken = default)
        {
            return _logic.IndexAsync(indexName, document, cancellationToken);
        }

        public Task IndexAsync(TDoc document, CancellationToken cancellationToken = default)
        {
            return IndexAsync(_indexNameProvider.Provide<TDoc>(), document, cancellationToken);
        }

        public IIndexSpecificEsIndexer<TDoc> ForIndex(string indexName)
        {
            return new IndexSpecificIndexer(indexName, _logic);
        }

        public IIndexSpecificEsIndexer<TDoc> ForIndex()
        {
            return ForIndex(_defaultIndexName);
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
            public Task IndexManyAsync(IEnumerable<TDoc> documents, CancellationToken cancellationToken = default)
            {
                return _logic.IndexManyAsync(IndexName, documents, cancellationToken);
            }

            public Task IndexAsync(TDoc document, CancellationToken cancellationToken = default)
            {
                return _logic.IndexAsync(IndexName, document, cancellationToken);
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