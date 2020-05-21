using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Extensions for 
    /// </summary>
    public static class EsManagerExtensions
    {
        public static async Task IndexManyAsync<TDoc>(this IEsManager mgr, string indexName, IEnumerable<TDoc> documents)
            where TDoc : class
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));
            var resIndexName = indexName ?? typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;

            var indexResponse = await mgr.Client.IndexManyAsync(documents, resIndexName);
            if (!indexResponse.IsValid)
                throw new EsIndexManyException(indexResponse);
        }

        public static Task IndexManyAsync<TDoc>(this IEsManager mgr, IEnumerable<TDoc> documents)
            where TDoc : class
        {
            return IndexManyAsync(mgr, null, documents);
        }

        public static async Task IndexAsync<TDoc>(this IEsManager mgr, string indexName, TDoc document)
            where TDoc : class
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var resIndexName = indexName ?? typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;

            var indexResponse = await mgr.Client.IndexAsync(document, iDesc => iDesc.Index(resIndexName));
            if (!indexResponse.IsValid)
                throw new EsIndexException(indexResponse);
        }

        public static Task IndexAsync<TDoc>(this IEsManager mgr, TDoc document)
            where TDoc : class
        {
            return IndexAsync(mgr, null, document);
        }

        public static async Task<IReadOnlyCollection<TDoc>> SearchAsync<TDoc>(
            this IEsManager mgr, 
            string indexName,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
            where TDoc : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var resIndexName = indexName ?? typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;

            var sr = await mgr.Client.SearchAsync<TDoc>(sd => sd
                .Index(resIndexName)
                .Query(query));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return sr.Documents;
        }

        public static Task<IReadOnlyCollection<TDoc>> SearchAsync<TDoc>(
            this IEsManager mgr,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
            where TDoc : class
        {
            return SearchAsync(mgr, null, query);
        }

        public static async Task<IReadOnlyCollection<HighLightedDocument<TDoc>>> SearchAsync<TDoc>(
            this IEsManager mgr,
            string indexName,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector)
            where TDoc : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (highlightSelector == null) throw new ArgumentNullException(nameof(highlightSelector));

            var resIndexName = indexName ?? typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;

            var sr = await mgr.Client.SearchAsync<TDoc>(sd => sd
                .Index(resIndexName)
                .Query(query)
                .Highlight(highlightSelector));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            var foundDocs = sr.Hits.Select(h => 
                new HighLightedDocument<TDoc>(
                    h.Source, 
                    h.Highlight
                        .Values
                        .FirstOrDefault()?
                        .FirstOrDefault()
                    )
            );

            return foundDocs.ToList().AsReadOnly();
        }

        public static Task<IReadOnlyCollection<HighLightedDocument<TDoc>>> SearchAsync<TDoc>(
            this IEsManager mgr,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector)
            where TDoc : class
        {
            return SearchAsync(mgr, null, query, highlightSelector);
        }
    }
}