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
        /// <summary>
        /// Adds ES tools services
        /// </summary>
        public static IServiceCollection AddEsTools(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            Configure(services, configuration);

            services.AddSingleton<IEsManager, DefaultEsManager>();

            return services;
        }

        private static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ElasticSearch");
            if(!config.Exists())
                throw new InvalidOperationException("Configuration section 'ElasticSearch' not found");

            if (!string.IsNullOrEmpty(config.Value))
            {
                services.Configure<ElasticSearchOptions>(opt =>
                {
                    opt.Url = config.Value;
                });
            }
            else
            {
                services.Configure<ElasticSearchOptions>(config);
            }
        }
    }

    /// <summary>
    /// Contains options parameters for ES tools
    /// </summary>
    public class ElasticSearchOptions
    {
        /// <summary>
        /// Base address
        /// </summary>
        public string Url { get; set; }
    }
}
