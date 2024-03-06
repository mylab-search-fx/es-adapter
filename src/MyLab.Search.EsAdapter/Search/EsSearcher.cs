using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Default implementation for <see cref="IEsSearcher"/>
    /// </summary>
    public class EsSearcher : IEsSearcher
    {
        private readonly IEsClientProvider _clientProvider;
        private readonly IEsResponseValidator _responseValidator;

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearcher"/>
        /// </summary>
        public EsSearcher(IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _clientProvider = clientProvider;
            _responseValidator = responseValidator;
        }

        /// <inheritdoc />
        public async Task<EsFound<TDoc>> SearchAsync<TDoc>(
            string indexName, 
            EsSearchParams<TDoc> searchParams, 
            CancellationToken cancellationToken = default) 
            where TDoc : class
        {
            var resp = await _clientProvider.Provide().SearchAsync(GetSearchFunc(indexName, searchParams, null), cancellationToken);

            _responseValidator.Validate(resp);

            return EsFound<TDoc>.FromSearchResponse(resp);
        }

        /// <inheritdoc />
        public async Task<EsHlFound<TDoc>> SearchAsync<TDoc>(
            string indexName, 
            EsSearchParams<TDoc> searchParams, 
            EsHlSelector<TDoc> highlight,
            CancellationToken cancellationToken = default) 
            where TDoc : class
        {
            var resp = await _clientProvider.Provide().SearchAsync(GetSearchFunc(indexName, searchParams, highlight), cancellationToken);

            _responseValidator.Validate(resp);

            return EsHlFound<TDoc>.FromSearchResponse(resp);
        }

        Func<SearchDescriptor<TDoc>, ISearchRequest> GetSearchFunc<TDoc>(
            string indexName, 
            EsSearchParams<TDoc> searchParams, 
            EsHlSelector<TDoc> highlight)
            where TDoc : class
        {
            return sd =>
            {
                var s = sd.Index(indexName).Query(searchParams.Query);

                if (searchParams.Sort != null)
                    s = s.Sort(searchParams.Sort);
                if (searchParams.Paging != null)
                    s = s.From(searchParams.Paging.From).Size(searchParams.Paging.Size);
                if (highlight != null)
                    s = s.Highlight(d => highlight(d));

                return s;
            };
        }
    }
}