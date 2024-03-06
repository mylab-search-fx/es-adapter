using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace MyLab.Search.EsAdapter.Inter
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
        public NewtonJsonEsSerializer(JsonSerializer jsonSerializer)
        {
            if (jsonSerializer == null) throw new ArgumentNullException(nameof(jsonSerializer));

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
        public Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            var res = _newtonSerializer.Deserialize(jsonReader, type);

            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            var res = _newtonSerializer.Deserialize<T>(jsonReader);

            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None)
        {
            TextWriter txtWriter = new StreamWriter(stream);
            JsonWriter jsonWriter = new JsonTextWriter(txtWriter)
            {
                Formatting = formatting == SerializationFormatting.Indented 
                    ? Formatting.Indented
                    : Formatting.None
            };

            _newtonSerializer.Serialize(jsonWriter, data);
            jsonWriter.Flush();
        }

        /// <inheritdoc />
        public async Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None,
            CancellationToken cancellationToken = default)
        {
            TextWriter txtWriter = new StreamWriter(stream);
            JsonWriter jsonWriter = new JsonTextWriter(txtWriter)
            {
                Formatting = formatting == SerializationFormatting.Indented
                    ? Formatting.Indented
                    : Formatting.None
            };

            _newtonSerializer.Serialize(jsonWriter, data);

            await jsonWriter.FlushAsync(cancellationToken);
        }
    }
}