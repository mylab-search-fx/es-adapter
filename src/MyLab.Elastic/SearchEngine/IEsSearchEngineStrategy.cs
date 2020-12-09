using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyLab.Elastic.SearchEngine
{
    public interface IEsSearchEngineStrategy<TDoc>
        where TDoc : class
    {
        IEnumerable<IEsSearchFilter<TDoc>> GetPredefinedFilters();
        IEsSearchFilter<TDoc> GetFilterFromQueryWord(string queryWord);
        IEnumerable<Expression<Func<TDoc, long>>> GetNumProperties();
        IEnumerable<Expression<Func<TDoc, string>>> GetTermProperties();
        IEnumerable<Expression<Func<TDoc, string>>> GetTextProperties();
    }
}