using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLab.Elastic.SearchEngine
{
    public abstract class EsSearchQueryFilterExtractor<TDoc> : IEsSearchQueryFilterExtractor<TDoc>
        where TDoc : class
    {
        private readonly string _regexpPattern;

        /// <summary>
        /// Initializes a new instance of <see cref="EsSearchQueryFilterExtractor{TDoc}"/>
        /// </summary>
        protected EsSearchQueryFilterExtractor(string regexpPattern)
        {
            _regexpPattern = regexpPattern;
        }

        public IEnumerable<IEsSearchFilter<TDoc>> CreateEsSearchFilters(string queryStr)
        {
            if (string.IsNullOrWhiteSpace(queryStr))
                return Enumerable.Empty<IEsSearchFilter<TDoc>>();

            var matches = Regex.Matches(queryStr, _regexpPattern);

            return matches.Where(m => m.Success).Select(CreateFilter);
        }

        protected abstract IEsSearchFilter<TDoc> CreateFilter(Match queryMatch);
    }
}