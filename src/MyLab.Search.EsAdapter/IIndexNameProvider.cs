using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using MyLab.Log;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Provides index name
    /// </summary>
    public interface IIndexNameProvider
    {
        /// <summary>
        /// Provides index name for document type
        /// </summary>
        string Provide<TDoc>();
    }

    /// <summary>
    /// Provides index name from options
    /// </summary>
    public class OptionsIndexNameProvider : IIndexNameProvider
    {
        private readonly EsOptions _options;

        /// <summary>
        /// Initializes a new instance of <see cref="OptionsIndexNameProvider"/>
        /// </summary>
        public OptionsIndexNameProvider(IOptions<EsOptions> options)
            :this(options?.Value)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OptionsIndexNameProvider"/>
        /// </summary>
        public OptionsIndexNameProvider(EsOptions options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public string Provide<TDoc>()
        {
            IndexBinding found = null;

            var docType = typeof(TDoc);
            EsBindingKeyAttribute keyAttr = null;

            if (_options?.IndexBindings != null)
            {
                keyAttr = docType.GetCustomAttribute<EsBindingKeyAttribute>();

                found = _options.IndexBindings.FirstOrDefault(
                    b =>
                        (keyAttr != null && b.Doc == keyAttr.Key) ||
                        b.Doc == docType.FullName ||
                        b.Doc == docType.Name);
            }

            if (found == null)
            {
                throw new InvalidOperationException("Document to index binding not found")
                    .AndFactIs("doc-type", docType.FullName)
                    .AndFactIs("binding-key", keyAttr != null ? keyAttr.Key : "[undefined]");
            }

            return found.Index;
        }
    }

    /// <summary>
    /// Provides single index name
    /// </summary>
    public class SingleIndexNameProvider : IIndexNameProvider
    {
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of <see cref="SingleIndexNameProvider"/>
        /// </summary>
        public SingleIndexNameProvider(string indexName)
        {
            _indexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
        }

        /// <inheritdoc />
        public string Provide<TDoc>()
        {
            return _indexName;
        }
    }
}
