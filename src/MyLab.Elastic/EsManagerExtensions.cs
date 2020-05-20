using System;
using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Extensions for 
    /// </summary>
    public static class EsManagerExtensions
    {
        public static Task IndexAsync<TDoc>(this IEsManager mgr, TDoc document)
            where TDoc : class
        {
            return Task.CompletedTask;
        }

        public static Task<TDoc> SearchAsync<TDoc>(
            this IEsManager mgr,
            Func<SearchDescriptor<TDoc>, ISearchRequest> query,
            Func<HighlightDescriptor<TDoc>, IHighlight> highlightSelector = null)
            where TDoc : class
        {
            return Task.FromResult(default(TDoc));
        }
    }
}