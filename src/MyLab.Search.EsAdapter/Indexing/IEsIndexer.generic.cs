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
    /// <typeparam name="TDoc">Document type</typeparam>
    public interface IEsIndexer<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Creates new document in the index
        /// </summary>
        Task CreateAsync(TDoc doc, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates or replaces document in the index
        /// </summary>
        Task IndexAsync(TDoc doc, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update indexed document partially
        /// </summary>
        Task UpdateAsync(Id id, Expression<Func<TDoc>> factoryExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update indexed document partially
        /// </summary>
        Task UpdateAsync(TDoc partialDocument, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes indexed document
        /// </summary>
        Task DeleteAsync(Id docId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Process bulk indexing request
        /// </summary>
        Task BulkAsync(EsBulkIndexingRequest<TDoc> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Process bulk indexing request
        /// </summary>
        Task BulkAsync(Func<BulkDescriptor, IBulkRequest> selector, CancellationToken cancellationToken = default);
    }
}
