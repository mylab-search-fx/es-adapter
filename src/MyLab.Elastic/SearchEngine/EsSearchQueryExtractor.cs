using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLab.Elastic.SearchEngine
{
    public abstract class EsSearchQueryExtractor<TDoc> : IEsSearchQueryFilterExtractor<TDoc>
        where TDoc : class
    {
        private readonly string _regexpPattern;

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearchQueryExtractor{TDoc}"/>
        /// </summary>
        protected EsSearchQueryExtractor(string regexpPattern)
        {
            _regexpPattern = regexpPattern;
        }

        public IEnumerable<IEsSearchFilter<TDoc>> CreateEsSearchFilters(string queryStr)
        {
            var matches = Regex.Matches(queryStr, _regexpPattern);

            return matches.Where(m => m.Success).Select(CreateFilter);
        }

        protected abstract IEsSearchFilter<TDoc> CreateFilter(Match queryMatch);
    }
}