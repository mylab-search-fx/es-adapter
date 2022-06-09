using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Default implementation for <see cref="IEsIndexTools{TDoc}"/>
    /// </summary>
    public class EsIndexTools<TDoc> : IEsIndexTools<TDoc> where TDoc: class
    {
        private readonly IEsIndexTools _baseIndexTools;
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsIndexTools(IEsIndexTools baseIndexTools, IOptions<EsOptions> options)
            :this(baseIndexTools, options.Value)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsIndexTools(IEsIndexTools baseIndexTools, EsOptions options)
            : this(baseIndexTools, new OptionsIndexNameProvider(options))
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsIndexTools(IEsIndexTools baseIndexTools, IIndexNameProvider indexNameProvider)
        {
            _baseIndexTools = baseIndexTools;
            _indexName = indexNameProvider.Provide<TDoc>();
        }

        /// <inheritdoc />
        public Task<IIndexDeleter> CreateIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.CreateIndexAsync(_indexName, createDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IIndexDeleter> CreateIndexAsync(string jsonSettings, CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.CreateIndexAsync(_indexName, jsonSettings, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteIndexAsync(CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.DeleteIndexAsync(_indexName, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> IsIndexExistsAsync(CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.IsIndexExistsAsync(_indexName, cancellationToken);
        }

        /// <inheritdoc />
        public Task PruneIndexAsync(CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.PruneIndexAsync(_indexName, cancellationToken);
        }
    }
}