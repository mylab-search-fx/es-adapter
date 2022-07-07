using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter.Indexing
{
    /// <summary>
    /// Indexes a document
    /// </summary>
    public interface IEsIndexer 
    {
        /// <summary>
        /// Creates new document in the index
        /// </summary>
        Task CreateAsync<TDoc>(string indexName, TDoc doc, CancellationToken cancellationToken = default)
            where TDoc : class;
        /// <summary>
        /// Creates or replaces document in the index
        /// </summary>
        Task IndexAsync<TDoc>(string indexName, TDoc doc, CancellationToken cancellationToken = default)
            where TDoc : class;

        /// <summary>
        /// Update indexed document partially
        /// </summary>
        Task UpdateAsync<TDoc>(string indexName, Id id, Expression<Func<TDoc>> factoryExpression, CancellationToken cancellationToken = default)
            where TDoc : class;

        /// <summary>
        /// Update indexed document partially
        /// </summary>
        Task UpdateAsync<TDoc>(string indexName, TDoc partialDocument, CancellationToken cancellationToken = default)
            where TDoc : class;

        /// <summary>
        /// Deletes indexed document
        /// </summary>
        Task DeleteAsync(string indexName, Id docId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Process bulk indexing request
        /// </summary>
        Task BulkAsync<TDoc>(string indexName, EsBulkIndexingRequest<TDoc> request, CancellationToken cancellationToken = default) where TDoc : class;

        /// <summary>
        /// Process bulk indexing request
        /// </summary>
        Task BulkAsync<TDoc>(string indexName, Func<BulkDescriptor, IBulkRequest> selector, CancellationToken cancellationToken = default) where TDoc : class;
    }
}
