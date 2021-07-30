using System.Text.RegularExpressions;

namespace MyLab.Search.EsAdapter.SearchEngine
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

        protected abstract IEsSearchFilter<TDoc> CreateFilter(Match queryMatch);
        public IEsSearchFilter<TDoc> CreateEsSearchFilter(string queryWord)
        {
            if (string.IsNullOrWhiteSpace(queryWord))
                return null;

            var match = Regex.Match(queryWord, _regexpPattern);

            return match.Success ? CreateFilter(match) : null;
        }
    }
}