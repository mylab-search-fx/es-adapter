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
        public EsHlFound(IList<HighLightedDocument<TDoc>> list) 
            : base(list)
        {
        }
    }

    /// <summary>
    /// Searching result
    /// </summary>
    public class EsFound<TDoc> : ReadOnlyCollection<TDoc>
        where TDoc : class
    {
        public EsFound(IList<TDoc> list)
            : base(list)
        {
        }
    }
}