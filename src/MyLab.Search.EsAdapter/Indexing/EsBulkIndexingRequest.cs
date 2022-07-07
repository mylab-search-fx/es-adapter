using Nest;

namespace MyLab.Search.EsAdapter.Indexing
{
    /// <summary>
    /// Contains data to make bulk indexing operation
    /// </summary>
    public class EsBulkIndexingRequest<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Contains documents for creating
        /// </summary>
        public TDoc[] CreateList { get; set; }
        /// <summary>
        /// Contains documents for indexing
        /// </summary>
        public TDoc[] IndexList { get; set; }
        /// <summary>
        /// Contains partial documents for updating
        /// </summary>
        public TDoc[] UpdateList { get; set; }
        /// <summary>
        /// Contains document identifiers for deleting
        /// </summary>
        public Id[] DeleteList { get; set; }

        /// <summary>
        /// Is request does not contains any data to indexing
        /// </summary>
        public bool IsEmpty()
        {
            return (CreateList == null || CreateList.Length == 0) &&
                   (IndexList == null || IndexList.Length == 0) &&
                   (UpdateList == null || UpdateList.Length == 0) &&
                   (DeleteList == null || DeleteList.Length == 0);
        }
    }
}