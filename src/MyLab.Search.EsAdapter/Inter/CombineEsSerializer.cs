using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Serializes user object with Newton.Json and core serializers
    /// </summary>
    public class CombineEsSerializer : IElasticsearchSerializer
    {
        private static readonly string NestLibName = typeof(Nest.AcknowledgeState).Assembly.GetName().Name;
        private static readonly string ElasticsearchLibName = typeof(Elasticsearch.Net.ApiCallDetails).Assembly.GetName().Name;

        private readonly JsonSerializer _newtonSerializer;
        private readonly IElasticsearchSerializer _coreSerializer;

        /// <summary>
        /// Initializes a new instance of <see cref="CombineEsSerializer"/>
        /// </summary>
        public CombineEsSerializer(IElasticsearchSerializer core)
        {
            _coreSerializer = core;
            _newtonSerializer = new JsonSerializer();
        }

        /// <inheritdoc />
        public object Deserialize(Type type, Stream stream)
        {
            if (IsCoreType(type))
                return _coreSerializer.Deserialize(type, stream);

            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            return _newtonSerializer.Deserialize(jsonReader, type);
        }

        /// <inheritdoc />
        public T Deserialize<T>(Stream stream)
        {
            if (IsCoreType(typeof(T)))
                return _coreSerializer.Deserialize<T>(stream);

            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            return _newtonSerializer.Deserialize<T>(jsonReader);
        }

        /// <inheritdoc />
        public Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            if (IsCoreType(type))
                return _coreSerializer.DeserializeAsync(type, stream, cancellationToken);

            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            var res = _newtonSerializer.Deserialize(jsonReader, type);

            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (IsCoreType(typeof(T)))
                return _coreSerializer.DeserializeAsync<T>(stream, cancellationToken);

            TextReader txtReader = new StreamReader(stream);
            JsonReader jsonReader = new JsonTextReader(txtReader);

            var res = _newtonSerializer.Deserialize<T>(jsonReader);

            return Task.FromResult(res);
        }

        /// <inheritdoc />
        public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None)
        {
            if (IsCoreType(typeof(T)))
            {
                _coreSerializer.Serialize<T>(data, stream, formatting);
                return;
            }

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
            if (IsCoreType(typeof(T)))
            {
                await _coreSerializer.SerializeAsync<T>(data, stream, formatting, cancellationToken);
                return;
            }

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

        bool IsCoreType(Type type)
        {
            var assName = type.Assembly.GetName().Name;

            return assName == NestLibName || assName == ElasticsearchLibName;
        }
    }
}