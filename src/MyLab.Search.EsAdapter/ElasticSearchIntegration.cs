using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Contains extension method to elastic search tools integration
    /// </summary>
    public static class ElasticSearchIntegration
    {
        public const string DefaultConfigSectionNameOld = "Elasticsearch";

        public const string DefaultConfigSectionName = "ES";

        /// <summary>
        /// Adds ES tools services
        /// </summary>
        [Obsolete("Please use AddEsTools(this IServiceCollection services) and ConfigureEsTools(...) instead")]
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

        /// <summary>
        /// Adds ES tools services
        /// </summary>
        public static IServiceCollection AddEsTools(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IEsClientProvider, EsClientProvider>();
            services.AddSingleton<IEsManager, EsManager>();
            services.AddSingleton<IIndexNameProvider, IndexNameProvider>();
            services.AddSingleton(typeof(IEsSearcher<>), typeof(EsSearcher<>));
            services.AddSingleton(typeof(IEsIndexer<>), typeof(EsIndexer<>));

            return services;
        }

        /// <summary>
        /// Adds ES tools services
        /// </summary>
        public static IServiceCollection ConfigureEsTools(this IServiceCollection services, IConfiguration configuration, string sectionName = DefaultConfigSectionName)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var configSection = configuration.GetSection(sectionName);

            if ((configSection == null || !configSection.Exists()) && sectionName == DefaultConfigSectionNameOld)
            {
                configSection = configuration.GetSection(DefaultConfigSectionNameOld);
            }

            return services.Configure<ElasticsearchOptions>(configSection);
        }

        /// <summary>
        /// Adds ES tools services
        /// </summary>
        public static IServiceCollection ConfigureEsTools(this IServiceCollection services, Action<ElasticsearchOptions> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            return services.Configure(configureOptions);
        }
    }
}
