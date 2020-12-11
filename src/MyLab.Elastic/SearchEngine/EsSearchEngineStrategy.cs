using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Base implementation <see cref="IEsSearchEngineStrategy{TDoc}"/>
    /// </summary>
    public class EsSearchEngineStrategy<TDoc> : IEsSearchEngineStrategy<TDoc>
        where TDoc : class
    {
        readonly List<IEsSearchFilter<TDoc>> _filters = new List<IEsSearchFilter<TDoc>>();
        readonly List<Expression<Func<TDoc, string>>> _termProps = new List<Expression<Func<TDoc, string>>>();
        readonly List<Expression<Func<TDoc, string>>> _textProps = new List<Expression<Func<TDoc, string>>>();
        readonly List<Expression<Func<TDoc, long>>> _numProps = new List<Expression<Func<TDoc, long>>>();
        readonly List<IEsSearchQueryFilterExtractor<TDoc>> _filterExtractors = new List<IEsSearchQueryFilterExtractor<TDoc>>();

        public IEsSearchFilter<TDoc> DefaultFilter { get; protected set; }

        public IEsSearchSort<TDoc> DefaultSort { get; protected set; }

        /// <summary>
        /// Registered predefined filter for all searches
        /// </summary>
        protected void AddPredefinedFilter(IEsSearchFilter<TDoc> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            _filters.Add(filter);
        }

        /// <summary>
        /// Registers term model property fro exact search
        /// </summary>
        protected void AddTermProperty(Expression<Func<TDoc, string>> propProvider)
        {
            if (propProvider == null) throw new ArgumentNullException(nameof(propProvider));

            _termProps.Add(propProvider);
        }

        /// <summary>
        /// Registers text model property for inclusion search
        /// </summary>
        protected void AddTextProperty(Expression<Func<TDoc, string>> propProvider)
        {
            if (propProvider == null) throw new ArgumentNullException(nameof(propProvider));

            _textProps.Add(propProvider);
        }

        /// <summary>
        /// Registers numeric model property for inclusion search
        /// </summary>
        protected void AddNumProperty(Expression<Func<TDoc, long>> propProvider)
        {
            if (propProvider == null) throw new ArgumentNullException(nameof(propProvider));

            _numProps.Add(propProvider);
        }

        /// <summary>
        /// Registers filter extractors
        /// </summary>
        protected void AddFilterExtractor(IEsSearchQueryFilterExtractor<TDoc> filterExtractor)
        {
            if (filterExtractor == null) throw new ArgumentNullException(nameof(filterExtractor));
            _filterExtractors.Add(filterExtractor);
        }

        public IEnumerable<IEsSearchFilter<TDoc>> GetPredefinedFilters()
        {
            return _filters.ToArray();
        }

        public IEsSearchFilter<TDoc> GetFilterFromQueryWord(string queryWord)
        {
            foreach (var filterExtractor in _filterExtractors)
            {
                var filter = filterExtractor.CreateEsSearchFilter(queryWord);
                if (filter != null)
                    return filter;
            }

            return null;
        }

        public IEnumerable<Expression<Func<TDoc, long>>> GetNumProperties()
        {
            return _numProps.ToArray();
        }

        public IEnumerable<Expression<Func<TDoc, string>>> GetTermProperties()
        {
            return _termProps.ToArray();
        }

        public IEnumerable<Expression<Func<TDoc, string>>> GetTextProperties()
        {
            return _textProps.ToArray();
        }
    }
}
