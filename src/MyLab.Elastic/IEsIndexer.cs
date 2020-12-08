using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyLab.Elastic
{
    /// <summary>
    /// Describes Elasticsearch Indexer
    /// </summary>
    public interface IEsIndexer<in TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Index document batch in specified index
        /// </summary>
        Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents);

        /// <summary>
        /// Index document batch in index which bound to document model
        /// </summary>
        Task IndexManyAsync(IEnumerable<TDoc> documents);

        /// <summary>
        /// Index document in specified index
        /// </summary>
        Task IndexAsync(string indexName, TDoc document);

        /// <summary>
        /// Index document in index which bound to document model
        /// </summary>
        Task IndexAsync(TDoc document);

        /// <summary>
        /// Creates index specific indexer
        /// </summary>
        IIndexSpecificEsIndexer<TDoc> ForIndex(string indexName);
    }

    /// <summary>
    /// Index document in specific index
    /// </summary>
    public interface IIndexSpecificEsIndexer<in TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Gets specific index name
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Index document batch in index which bound to document model
        /// </summary>
        Task IndexManyAsync(IEnumerable<TDoc> documents);
        
        /// <summary>
        /// Index document in index which bound to document model
        /// </summary>
        Task IndexAsync(TDoc document);
    }
}