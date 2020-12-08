using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.Elastic
{
    /// <summary>
    /// Contains extension method to elastic search tools integration
    /// </summary>
    public static class ElasticSearchIntegration
    {
        public const string DefaultConfigSectionName = "Elasticsearch";

        /// <summary>
        /// Adds ES tools services
        /// </summary>
        public static IServiceCollection AddEsTools(this IServiceCollection services, IConfiguration configuration, string sectionName = DefaultConfigSectionName)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<IEsClientProvider, EsClientProvider>();
            services.AddSingleton<IEsManager, EsManager>();
            services.AddSingleton<IIndexNameProvider, IndexNameProvider>();
            services.AddSingleton(typeof(IEsSearcher<>), typeof(EsSearcher<>));
            services.AddSingleton(typeof(IEsIndexer<>), typeof(EsIndexer<>));
            services.Configure<ElasticsearchOptions>(configuration.GetSection(sectionName));

            return services;
        }
    }
}
