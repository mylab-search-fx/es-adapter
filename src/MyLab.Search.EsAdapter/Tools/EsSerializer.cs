using System;
using System.IO;
using System.Text;
using Elasticsearch.Net;
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

        public LifecyclePolicy DeserializeLifecyclePolicy(Stream stream)
        {
            return _client.SourceSerializer.Deserialize<LifecyclePolicy>(stream);
        }

        public IndexTemplate DeserializeIndexTemplate(Stream stream)
        {
            return _client.SourceSerializer.Deserialize<IndexTemplate>(stream);
        }

        public ComponentTemplate DeserializeComponentTemplate(Stream stream)
        {
            return _client.SourceSerializer.Deserialize<ComponentTemplate>(stream);
        }

        public void SerializeLifecyclePolicy(LifecyclePolicy policy, Stream stream)
        {
            _client.SourceSerializer.Serialize(policy, stream);
        }

        public void SerializeIndexTemplate(IndexTemplate template, Stream stream)
        {
            _client.SourceSerializer.Serialize(template, stream);
        }

        public void SerializeComponentTemplate(ComponentTemplate template, Stream stream)
        {
            _client.SourceSerializer.Serialize(template, stream);
        }
    }
}