﻿using System;
using System.IO;
using System.Text;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    class EsDeserializer : IEsDeserializer
    {
        private readonly ElasticClient _client;

        public EsDeserializer(IEsClientProvider clientProvider)
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
    }
}