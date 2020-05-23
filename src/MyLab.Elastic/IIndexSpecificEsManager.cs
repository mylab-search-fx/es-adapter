using System;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Represents index specific manager
    /// </summary>
    public interface IIndexSpecificEsManager
    {
        /// <summary>
        /// Context index name
        /// </summary>
        public string IndexName { get; }

        /// <summary>
        /// ES NEST client
        /// </summary>
        ElasticClient Client { get; }
    }

    public class DefaultIndexSpecificEsManager : IIndexSpecificEsManager
    {
        /// <inheritdoc />
        public string IndexName { get; set; }

        /// <inheritdoc />
        public ElasticClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultIndexSpecificEsManager"/>
        /// </summary>
        public DefaultIndexSpecificEsManager(string indexName, ElasticClient elasticClient)
        {
            IndexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
            Client = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }
    }
}