using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.Elastic
{
    /// <summary>
    /// Highlight searching result
    /// </summary>
    public class EsHlFound<TDoc> : ReadOnlyCollection<HighLightedDocument<TDoc>>
        where TDoc : class
    {
        public long Total { get; }

        public EsHlFound(IList<HighLightedDocument<TDoc>> list, long total) 
            : base(list)
        {
            Total = total;
        }
    }

    /// <summary>
    /// Searching result
    /// </summary>
    public class EsFound<TDoc> : ReadOnlyCollection<TDoc>
        where TDoc : class
    {
        public long Total { get; }

        public EsFound(IList<TDoc> list, long total)
            : base(list)
        {
            Total = total;
        }
    }
}