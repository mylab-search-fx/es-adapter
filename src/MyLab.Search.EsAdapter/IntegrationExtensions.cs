using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Search;
using MyLab.Search.EsAdapter.Tools;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Contains extension methods to integrate ES adapter services
    /// </summary>
    public static class IntegrationExtensions
    {
        /// <summary>
        /// Default config section name
        /// </summary>
        public const string DefaultSectionName = "ES";

        /// <summary>
        /// Adds ES adapter services into service collection
        /// </summary>
        public static IServiceCollection AddEsTools(this IServiceCollection srv, Action<EsOptions> configure = null)
        {
            if (srv == null) throw new ArgumentNullException(nameof(srv));

            srv
                .AddSingleton<IEsClientProvider, EsClientProvider>()
                .AddSingleton<IEsTools, EsTools>()
                .AddSingleton<IEsIndexer, EsIndexer>()
                .AddSingleton<IEsSearcher, EsSearcher>()
                .AddSingleton(typeof(IEsIndexer<>), typeof(EsIndexer<>))
                .AddSingleton(typeof(IEsSearcher<>), typeof(EsSearcher<>))
                .AddSingleton<IEsResponseValidator, EsResponseValidator>();

            if (configure != null)
                srv.Configure(configure);

            return srv;
        }

        /// <summary>
        /// Configures ES adapter services with action
        /// </summary>
        public static IServiceCollection ConfigureEsTools(this IServiceCollection srv, Action<EsOptions> configure)
        {
            if (srv == null) throw new ArgumentNullException(nameof(srv));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            return srv.Configure(configure);
        }

        /// <summary>
        /// Configures ES adapter services with config section
        /// </summary>
        public static IServiceCollection ConfigureEsTools(this IServiceCollection srv, IConfiguration config, string sectionName = DefaultSectionName)
        {
            if (srv == null) throw new ArgumentNullException(nameof(srv));
            if (config == null) throw new ArgumentNullException(nameof(config));

            return srv.Configure<EsOptions>(config.GetSection(sectionName));
        }
    }
}
