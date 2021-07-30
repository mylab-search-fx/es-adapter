using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Search.EsAdapter
{
    class EsLogic<TDoc> where TDoc : class
    {
        private readonly ElasticClient _cl;

        public EsLogic(ElasticClient cl)
        {
            _cl = cl;
        }

        public Task UpdateAsync(string indexName, string docId, Expression<Func<TDoc>> updateExpression, CancellationToken cancellationToken)
        {
            if (updateExpression == null) throw new ArgumentNullException(nameof(updateExpression));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexName));
            if (string.IsNullOrWhiteSpace(docId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(docId));

            return CoreUpdateAsync(indexName, new Id(docId), new UpdateDocument<TDoc>(updateExpression), cancellationToken);
        }

        public Task UpdateAsync(string indexName, long docId, Expression<Func<TDoc>> updateExpression, CancellationToken cancellationToken)
        {
            if (updateExpression == null) throw new ArgumentNullException(nameof(updateExpression));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexName));

            return CoreUpdateAsync(indexName, new Id(docId), new UpdateDocument<TDoc>(updateExpression),  cancellationToken);
        }

        public async Task IndexManyAsync(string indexName, IEnumerable<TDoc> documents, CancellationToken cancellationToken)
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexName));

            var indexResponse = await _cl.IndexManyAsync(documents, indexName, cancellationToken);
            if (!indexResponse.IsValid)
                throw new EsIndexManyException(indexResponse);
        }

        public async Task IndexAsync(string indexName, TDoc document, CancellationToken cancellationToken)
            
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexName));

            var indexResponse = await _cl.IndexAsync(document, iDesc => iDesc.Index(indexName), cancellationToken);
            if (!indexResponse.IsValid)
                throw new EsIndexException(indexResponse);
        }

        public async Task<EsFound<TDoc>> SearchAsync(string indexName, SearchParams<TDoc> searchParams, CancellationToken cancellationToken)
        {
            if (searchParams == null) throw new ArgumentNullException(nameof(searchParams));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexName));

            var sr = await _cl.SearchAsync(GetSearchFunc(indexName, searchParams), cancellationToken);

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return new EsFound<TDoc>(sr.Documents.ToList(), sr.Total);
        }

        public async Task<EsHlFound<TDoc>> SearchAsync(
            string indexName, 
            SearchParams<TDoc> searchParams,
            EsHlSelector<TDoc> highlight, CancellationToken cancellationToken)
        {
            if (searchParams == null) throw new ArgumentNullException(nameof(searchParams));
            if (highlight == null) throw new ArgumentNullException(nameof(highlight));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexName));

            var sr = await _cl.SearchAsync(GetSearchFunc(indexName, searchParams, highlight), cancellationToken);

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

        Task CoreUpdateAsync(string indexName, Id docId, UpdateDocument<TDoc> updateDocument, CancellationToken cancellationToken)
        {
            var updateReq = new UpdateRequest<TDoc, dynamic>(indexName, docId)
            {
                Doc = updateDocument.ToUpdateModel()
            };

            return _cl.UpdateAsync(updateReq, cancellationToken);
        }
    }
}
