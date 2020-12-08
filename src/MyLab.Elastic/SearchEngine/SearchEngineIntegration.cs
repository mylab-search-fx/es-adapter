using System;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.Elastic.SearchEngine
{
    /// <summary>
    /// Integration methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class SearchEngineIntegration
    {
        /// <summary>
        /// Adds search engine for model <see cref="TDoc"/>
        /// </summary>
        public static IServiceCollection AddEsSearchEngine<TSearchEngine, TDoc>(this IServiceCollection services)
            where TSearchEngine : class, IEsSearchEngine<TDoc>
            where TDoc: class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return services.AddSingleton<IEsSearchEngine<TDoc>, TSearchEngine>();
        }
    }
}
