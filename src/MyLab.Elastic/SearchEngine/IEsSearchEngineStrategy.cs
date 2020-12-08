using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyLab.Elastic.SearchEngine
{
    public interface IEsSearchEngineStrategy<TDoc>
        where TDoc : class
    {
        IEnumerable<IEsSearchFilter<TDoc>> GetPredefinedFilters();
        IEnumerable<IEsSearchFilter<TDoc>> GetFiltersFromQuery(string queryStr);
        IEnumerable<Expression<Func<TDoc, long>>> GetNumProperties();
        IEnumerable<Expression<Func<TDoc, string>>> GetTermProperties();
        IEnumerable<Expression<Func<TDoc, string>>> GetTextProperties();
    }
}