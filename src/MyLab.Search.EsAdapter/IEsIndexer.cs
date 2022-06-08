using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter
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
    }

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
    }
}
