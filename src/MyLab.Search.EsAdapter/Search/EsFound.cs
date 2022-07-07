using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Nest;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Searching result
    /// </summary>
    public class EsFound<TDoc> : ReadOnlyCollection<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Total found docs without paging
        /// </summary>
        public long Total { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsHlFound{TDoc}"/>
        /// </summary>
        public EsFound(IList<TDoc> list, long total)
            : base(list)
        {
            Total = total;
        }

        /// <summary>
        /// Create From search response
        /// </summary>
        public static EsFound<TDoc> FromSearchResponse(ISearchResponse<TDoc> response)
        {
            return new EsFound<TDoc>(response.Documents.ToList(), response.Total);
        }
    }
}