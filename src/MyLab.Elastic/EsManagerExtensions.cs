using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Extensions for <see cref="IEsManager"/>
    /// </summary>
    public static class EsManagerExtensions
    {
        /// <summary>
        /// Index document batch in specified index
        /// </summary>
        public static async Task IndexManyAsync<TDoc>(this IEsManager mgr, string indexName, IEnumerable<TDoc> documents)
            where TDoc : class
        {
            if (mgr == null) throw new ArgumentNullException(nameof(mgr));

            await new EsLogic<TDoc>(mgr.Client)
                .IndexManyAsync(indexName, documents);
        }

        /// <summary>
        /// Index document batch in index which bound to document model
        /// </summary>
        public static Task IndexManyAsync<TDoc>(this IEsManager mgr, IEnumerable<TDoc> documents)
            where TDoc : class
        {
            return IndexManyAsync(mgr, null, documents);
        }

        /// <summary>
        /// Index document in specified index
        /// </summary>
        public static async Task IndexAsync<TDoc>(this IEsManager mgr, string indexName, TDoc document)
            where TDoc : class
        {
            if (mgr == null) throw new ArgumentNullException(nameof(mgr));

            await new EsLogic<TDoc>(mgr.Client)
                .IndexAsync(indexName, document);
        }

        /// <summary>
        /// Index document in index which bound to document model
        /// </summary>
        public static Task IndexAsync<TDoc>(this IEsManager mgr, TDoc document)
            where TDoc : class
        {
            return IndexAsync(mgr, null, document);
        }

        /// <summary>
        /// Search documents in specified index
        /// </summary>
        public static async Task<IReadOnlyCollection<TDoc>> SearchAsync<TDoc>(
            this IEsManager mgr, 
            string indexName,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
            where TDoc : class
        {
            if (mgr == null) throw new ArgumentNullException(nameof(mgr));

            return await new EsLogic<TDoc>(mgr.Client)
                .SearchAsync(indexName, query);
        }

        /// <summary>
        /// Search documents in index which bound to document model
        /// </summary>
        public static Task<IReadOnlyCollection<TDoc>> SearchAsync<TDoc>(
            this IEsManager mgr,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
            where TDoc : class
        {
            return SearchAsync(mgr, null, query);
        }

        /// <summary>
        /// Search documents with highlighting in specified index
        /// </summary>
        public static async Task<IReadOnlyCollection<HighLightedDocument<TDoc>>> SearchAsync<TDoc>(
            this IEsManager mgr,
            string indexName,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector)
            where TDoc : class
        {
            if (mgr == null) throw new ArgumentNullException(nameof(mgr));

            return await new EsLogic<TDoc>(mgr.Client)
                .SearchAsync(indexName, query, highlightSelector);
        }

        /// <summary>
        /// Search documents with highlighting in index which bound to document model
        /// </summary>
        public static Task<IReadOnlyCollection<HighLightedDocument<TDoc>>> SearchAsync<TDoc>(
            this IEsManager mgr,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector)
            where TDoc : class
        {
            return SearchAsync(mgr, null, query, highlightSelector);
        }

        /// <summary>
        /// Create index specific manager
        /// </summary>
        public static IIndexSpecificEsManager ForIndex(this IEsManager mgr, string indexName)
        {
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (mgr == null) throw new ArgumentNullException(nameof(mgr));

            return new DefaultIndexSpecificEsManager(indexName, mgr.Client);
        }
    }
}