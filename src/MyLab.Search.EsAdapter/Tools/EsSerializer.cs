using System;
using System.IO;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    class EsSerializer : IEsSerializer
    {
        private readonly ElasticClient _client;

        public EsSerializer(IEsClientProvider clientProvider)
        {
            if (clientProvider == null) throw new ArgumentNullException(nameof(clientProvider));

            _client = clientProvider.Provide();
        }

        public T Deserialize<T>(Stream stream)
        {
            return _client.SourceSerializer.Deserialize<T>(stream);
        }

        public void Serialize(object obj, Stream stream)
        {
            _client.SourceSerializer.Serialize(obj, stream);
        }
    }
}