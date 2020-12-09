using System.Collections.Generic;
using Nest;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Provides sortKey function
    /// </summary>
    public interface IEsSearchSort<TDoc>
        where TDoc : class
    {
        IPromise<IList<ISort>>  Sort(SortDescriptor<TDoc> d);
    }
}