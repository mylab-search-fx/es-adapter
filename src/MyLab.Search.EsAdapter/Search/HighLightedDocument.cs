using System.Collections.Generic;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Contains document and
    /// </summary>
    public class HighLightedDocument<TDoc>
    {
        /// <summary>
        /// Biz document
        /// </summary>
        public TDoc Doc { get; }

        /// <summary>
        /// Highlights string
        /// </summary>
        public IDictionary<string,string> Highlights { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HighLightedDocument{TDoc}"/>
        /// </summary>
        public HighLightedDocument(TDoc doc, IDictionary<string, string> highlight)
        {
            Doc = doc;
            Highlights = highlight;
        }
    }
}