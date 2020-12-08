using System.Threading.Tasks;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Provides manager ES functions
    /// </summary>
    public interface IEsManager
    {
        Task<bool> PingAsync();
    }

    class EsManager : IEsManager
    {
        private readonly ElasticClient _client;

        public EsManager(IEsClientProvider clientProvider)
        {
            _client = clientProvider.Provide();
        }

        public async Task<bool> PingAsync()
        {
            var resp = await _client.PingAsync();

            return resp.IsValid;
        }
    }
}
