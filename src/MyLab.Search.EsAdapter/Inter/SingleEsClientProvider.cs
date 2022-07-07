using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Provides single <see cref="ElasticClient"/>
    /// </summary>
    public class SingleEsClientProvider : IEsClientProvider
    {
        private readonly ElasticClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="SingleEsClientProvider"/>
        /// </summary>
        public SingleEsClientProvider(ElasticClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ElasticClient Provide()
        {
            return _client;
        }
    }
}