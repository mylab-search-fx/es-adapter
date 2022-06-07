using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace MyLab.Search.EsAdapter.Serialization
{
    /// <summary>
    /// Serializes user object with Newton.Json
    /// </summary>
    public class NewtonJsonEsSerializer : IElasticsearchSerializer
    {
        private readonly JsonSerializer _newtonSerializer;

        /// <summary>
        /// Initializes a new instance of <see cref="NewtonJsonEsSerializer"/>
        /// </summary>
        public NewtonJsonEsSerializer()
        {
            _newtonSerializer = new JsonSerializer();
        }

        /// <inheritdoc />
        public object Deserialize(Type type, Stream stream)
        {
            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            return _newtonSerializer.Deserialize(jsonReader, type);
        }

        /// <inheritdoc />
        public T Deserialize<T>(Stream stream)
        {
            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            return _newtonSerializer.Deserialize<T>(jsonReader);
        }

        /// <inheritdoc />
        public Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = new CancellationToken())
        {
            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            var res = _newtonSerializer.Deserialize(jsonReader, type);

            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = new CancellationToken())
        {
            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            var res = _newtonSerializer.Deserialize<T>(jsonReader);

            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None)
        {
            using TextWriter txtWriter = new StreamWriter(stream);
            using JsonWriter jsonWriter = new JsonTextWriter(txtWriter)
            {
                Formatting = formatting == SerializationFormatting.Indented 
                    ? Formatting.Indented
                    : Formatting.None
            };

            _newtonSerializer.Serialize(jsonWriter, data);
        }

        /// <inheritdoc />
        public Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None,
            CancellationToken cancellationToken = new CancellationToken())
        {
            using TextWriter txtWriter = new StreamWriter(stream);
            using JsonWriter jsonWriter = new JsonTextWriter(txtWriter)
            {
                Formatting = formatting == SerializationFormatting.Indented
                    ? Formatting.Indented
                    : Formatting.None
            };

            _newtonSerializer.Serialize(jsonWriter, data);

            return Task.CompletedTask;
        }
    }
}