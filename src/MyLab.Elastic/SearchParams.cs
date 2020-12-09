using System;
using System.Collections.Generic;
using Nest;

namespace MyLab.Elastic
{
    public class SearchParams<TDoc>
        where TDoc : class
    {
        public Func<QueryContainerDescriptor<TDoc>, QueryContainer> Query { get; }

        public  Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> Sort { get; set; }

        public EsPaging Page { get; set; }

        public SearchParams(Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public static implicit operator SearchParams<TDoc>(Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            return new SearchParams<TDoc>(query);
        }
    }
}