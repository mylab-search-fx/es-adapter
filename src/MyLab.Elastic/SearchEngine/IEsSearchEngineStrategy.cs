using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyLab.Elastic.SearchEngine
{
    public interface IEsSearchEngineStrategy<TDoc>
        where TDoc : class
    {
        /// <summary>
        /// Uses when no named filter specified
        /// </summary>
        IEsSearchFilter<TDoc> DefaultFilter { get; }

        /// <summary>
        /// Uses when no named sort specified
        /// </summary>
        IEsSearchSort<TDoc> DefaultSort { get; }
        IEnumerable<IEsSearchFilter<TDoc>> GetPredefinedFilters();
        IEsSearchFilter<TDoc> GetFilterFromQueryWord(string queryWord);
        IEnumerable<Expression<Func<TDoc, long>>> GetNumProperties();
        IEnumerable<Expression<Func<TDoc, string>>> GetTermProperties();
        IEnumerable<Expression<Func<TDoc, string>>> GetTextProperties();
    }
}