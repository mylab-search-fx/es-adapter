using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Extensions for <see cref="IIndexSpecificEsManager"/>
    /// </summary>
    public static class IndexSpecificEsManagerExtensions
    {
        /// <summary>
        /// Index document batch in specified index
        /// </summary>
        public static async Task IndexManyAsync<TDoc>(this IIndexSpecificEsManager mgr, IEnumerable<TDoc> documents)
            where TDoc : class
        {
            await new EsLogic<TDoc>(mgr.Client)
                .IndexManyAsync(mgr.IndexName, documents);
        }
        /// <summary>
        /// Index document in specified index
        /// </summary>
        public static async Task IndexAsync<TDoc>(this IIndexSpecificEsManager mgr, TDoc document)
            where TDoc : class
        {
            await new EsLogic<TDoc>(mgr.Client)
                .IndexAsync(mgr.IndexName, document);
        }

        /// <summary>
        /// Search documents in specified index
        /// </summary>
        public static async Task<IReadOnlyCollection<TDoc>> SearchAsync<TDoc>(
            this IIndexSpecificEsManager mgr,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
            where TDoc : class
        {
            return await new EsLogic<TDoc>(mgr.Client)
                .SearchAsync(mgr.IndexName, query);
        }

        /// <summary>
        /// Search documents with highlighting in specified index
        /// </summary>
        public static async Task<IReadOnlyCollection<HighLightedDocument<TDoc>>> SearchAsync<TDoc>(
            this IIndexSpecificEsManager mgr,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector)
            where TDoc : class
        {
            return await new EsLogic<TDoc>(mgr.Client)
                .SearchAsync(mgr.IndexName, query, highlightSelector);
        }
    }
}