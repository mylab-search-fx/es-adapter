using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Nest;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Highlight searching result
    /// </summary>
    public class EsHlFound<TDoc> : ReadOnlyCollection<HighLightedDocument<TDoc>>
        where TDoc : class
    {
        /// <summary>
        /// Total found docs without paging
        /// </summary>
        public long Total { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsHlFound{TDoc}"/>
        /// </summary>
        public EsHlFound(IList<HighLightedDocument<TDoc>> list, long total)
            : base(list)
        {
            Total = total;
        }

        /// <summary>
        /// Create From search response
        /// </summary>
        public static EsHlFound<TDoc> FromSearchResponse(ISearchResponse<TDoc> response)
        {
            var foundDocs = response.Hits.Select(h =>
                new HighLightedDocument<TDoc>(
                    h.Source,
                    h.Highlight.ToDictionary(
                            kv => kv.Key,
                            kv => kv.Value.FirstOrDefault()
                        )
                )
            );

            return new EsHlFound<TDoc>(foundDocs.ToList(), response.Total);
        }
    }
}