using System;
using System.Collections.Generic;
using Nest;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Contains search parameters
    /// </summary>
    public class EsSearchParams<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Search query
        /// </summary>
        public Func<QueryContainerDescriptor<TDoc>, QueryContainer> Query { get; }

        /// <summary>
        /// Sorting description
        /// </summary>
        public Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> Sort { get; set; }

        /// <summary>
        /// Paging
        /// </summary>
        public EsPaging Paging { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearchParams{TDoc}"/>
        /// </summary>
        public EsSearchParams(Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        /// <summary>
        /// Implicit convert NEST query into EsSearchParams
        /// </summary>
        public static implicit operator EsSearchParams<TDoc>(Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            return new EsSearchParams<TDoc>(query);
        }
    }
}