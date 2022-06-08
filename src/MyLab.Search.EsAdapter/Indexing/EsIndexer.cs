using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Indexing
{
    class EsIndexer : IEsIndexer
    {
        private readonly IEsClientProvider _clientProvider;

        public EsIndexer(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task CreateAsync<TDoc>(string indexName, TDoc doc, CancellationToken cancellationToken = default) where TDoc : class
        {
            var resp = await _clientProvider.Provide().CreateAsync(doc, d => d.Index(indexName), cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to create document");
        }

        public async Task IndexAsync<TDoc>(string indexName, TDoc doc, CancellationToken cancellationToken = default) where TDoc : class
        {
            var resp = await _clientProvider.Provide().IndexAsync(doc, d => d.Index(indexName), cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to index document");
        }

        public async Task UpdateAsync<TDoc>(string indexName, Id id, Expression<Func<TDoc>> factoryExpression, CancellationToken cancellationToken = default) where TDoc : class
        {
            var updateExpr = new UpdateExpression<TDoc>(factoryExpression);
            var updateReq = new UpdateRequest<TDoc, dynamic>(indexName, id)
            {
                Doc = updateExpr.ToUpdateModel()
            };

            var resp = await _clientProvider.Provide().UpdateAsync(updateReq, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to update document");
        }

        public async Task UpdateAsync<TDoc>(string indexName, TDoc partialDocument, CancellationToken cancellationToken = default) where TDoc : class
        {
            var resp = await _clientProvider.Provide().UpdateAsync<TDoc>(
                DocumentPath<TDoc>.Id(partialDocument), 
                d => d
                    .Index(indexName)
                    .Doc(partialDocument)
                    .DocAsUpsert(), 
                cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to update document");
        }

        public async Task DeleteAsync(string indexName, Id docId, CancellationToken cancellationToken = default)
        {
            IDeleteRequest deleteReq = new DeleteRequest(indexName, docId);
            var resp = await _clientProvider.Provide().DeleteAsync(deleteReq, cancellationToken);

            EsException.ThrowIfInvalid(resp, "Unable to delete document");
        }

        public async Task BulkAsync<TDoc>(string indexName, EsBulkIndexingRequest<TDoc> request, CancellationToken cancellationToken = default) where TDoc : class
        {
            if (request.IsEmpty())
                throw new InvalidOperationException("Bulk request is empty");

            IBulkRequest req = new BulkRequest(indexName)
            {
                Operations = new BulkOperationsCollection<IBulkOperation>()
            };

            if (request.CreateList != null)
                req.Operations.AddRange(request.CreateList.Select(d => new BulkCreateOperation<TDoc>(d)));
            if (request.IndexList != null)
                req.Operations.AddRange(request.IndexList.Select(d => new BulkIndexOperation<TDoc>(d)));
            if (request.UpdateList != null)
                req.Operations.AddRange(request.UpdateList.Select(d => new BulkUpdateOperation<TDoc, TDoc>(d)
                {
                    Doc = d,
                    DocAsUpsert = true
                }));
            if (request.DeleteList != null)
                req.Operations.AddRange(request.DeleteList.Select(d => new BulkDeleteOperation<TDoc>(d)));

            var resp = await _clientProvider.Provide().BulkAsync(req, cancellationToken);

            EsException.ThrowIfInvalid(resp);
        }
    }
}