using System;

namespace MyLab.Search.EsAdapter.SearchEngine
{
    public partial class EsSearchEngine<TDoc>
    {
        /// <summary>
        /// Uses when no named filter specified
        /// </summary>
        protected IEsSearchFilter<TDoc> DefaultFilter { get; set; }

        /// <summary>
        /// Uses when no named sort specified
        /// </summary>
        protected IEsSearchSort<TDoc> DefaultSort { get; set; }

        /// <summary>
        /// Registers filter with specified key
        /// </summary>
        protected void RegisterNamedFilter(string key, IEsSearchFilter<TDoc> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            _registeredFilters.Add(key, filter);
        }

        /// <summary>
        /// Registers sort with specified key
        /// </summary>
        protected void RegisterNamedSort(string key, IEsSearchSort<TDoc> sort)
        {
            if (sort == null) throw new ArgumentNullException(nameof(sort));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            _registeredSorts.Add(key, sort);
        }
    }
}