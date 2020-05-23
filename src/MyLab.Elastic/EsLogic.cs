using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    class EsLogic<TDoc> where TDoc : class
    {
        private readonly ElasticClient _cl;

        public EsLogic(ElasticClient cl)
        {
            _cl = cl;
        }

        string RetrieveIndexName(string specifiedName)
        {
            return specifiedName ?? typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
        }
        
        public async Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents)
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            var resIndexName = RetrieveIndexName(indexName);

            var indexResponse = await _cl.IndexManyAsync(documents, resIndexName);
            if (!indexResponse.IsValid)
                throw new EsIndexManyException(indexResponse);
        }

        public async Task IndexAsync(string indexName, TDoc document)
            
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var resIndexName = RetrieveIndexName(indexName);

            var indexResponse = await _cl.IndexAsync(document, iDesc => iDesc.Index(resIndexName));
            if (!indexResponse.IsValid)
                throw new EsIndexException(indexResponse);
        }

        public async Task<IReadOnlyCollection<TDoc>> SearchAsync(
            string indexName,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var resIndexName = RetrieveIndexName(indexName);

            var sr = await _cl.SearchAsync<TDoc>(sd => sd
                .Index(resIndexName)
                .Query(query));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return sr.Documents;
        }

        public async Task<IReadOnlyCollection<HighLightedDocument<TDoc>>> SearchAsync(
            string indexName,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (highlightSelector == null) throw new ArgumentNullException(nameof(highlightSelector));

            var resIndexName = RetrieveIndexName(indexName);

            var sr = await _cl.SearchAsync<TDoc>(sd => sd
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
    }
}
