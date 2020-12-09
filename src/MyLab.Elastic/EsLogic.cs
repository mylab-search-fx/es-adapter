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
        
        public async Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents)
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            var indexResponse = await _cl.IndexManyAsync(documents, indexName);
            if (!indexResponse.IsValid)
                throw new EsIndexManyException(indexResponse);
        }

        public async Task IndexAsync(string indexName, TDoc document)
            
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var indexResponse = await _cl.IndexAsync(document, iDesc => iDesc.Index(indexName));
            if (!indexResponse.IsValid)
                throw new EsIndexException(indexResponse);
        }

        public async Task<EsFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams)
        {
            if (searchParams == null) throw new ArgumentNullException(nameof(searchParams));

            var sr = await _cl.SearchAsync(GetSearchFunc(indexName, searchParams));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return new EsFound<TDoc>(sr.Documents.ToList(), sr.Total);
        }

        public async Task<EsHlFound<TDoc>> SearchAsync(
            string indexName, 
            SearchParams<TDoc> searchParams,
            EsHlSelector<TDoc> highlight)
        {
            if (searchParams == null) throw new ArgumentNullException(nameof(searchParams));
            if (highlight == null) throw new ArgumentNullException(nameof(highlight));

            var sr = await _cl.SearchAsync(GetSearchFunc(indexName, searchParams, highlight));

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

            return new EsHlFound<TDoc>(foundDocs.ToList(), sr.Total);
        }

        Func<SearchDescriptor<TDoc>, ISearchRequest> GetSearchFunc(string indexName, SearchParams<TDoc> searchParams, EsHlSelector<TDoc> highlight = null)
        {
            return sd =>
            {
                var s = sd.Index(indexName).Query(searchParams.Query);

                if (searchParams.Sort != null)
                    s = s.Sort(searchParams.Sort);
                if (searchParams.Page != null)
                    s = s.From(searchParams.Page.From).Size(searchParams.Page.Size);
                if (highlight != null)
                    s = s.Highlight(d => highlight(d));

                return s;
            };
        }
    }
}
