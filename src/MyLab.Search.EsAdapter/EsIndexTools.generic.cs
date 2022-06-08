using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter
{
    class EsIndexTools<TDoc> : IEsIndexTools<TDoc> where TDoc: class
    {
        private readonly IEsIndexTools _baseIndexTools;
        private readonly Lazy<string> _indexName;

        public EsIndexTools(IEsIndexTools baseIndexTools, IOptions<EsOptions> options)
            :this(baseIndexTools, options.Value)
        {
            
        }

        public EsIndexTools(IEsIndexTools baseIndexTools, EsOptions options)
        {
            _baseIndexTools = baseIndexTools;
            _indexName = new Lazy<string>(options.GetIndexForDocType<TDoc>);
        }

        public Task<IIndexDeleter> CreateIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexTools.CreateIndexAsync(indexName, createDescriptor, cancellationToken);
        }

        public Task<IIndexDeleter> CreateIndexAsync(string jsonSettings, CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexTools.CreateIndexAsync(indexName, jsonSettings, cancellationToken);
        }

        public Task DeleteIndexAsync(CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexTools.DeleteIndexAsync(indexName, cancellationToken);
        }

        public Task<bool> IsIndexExistsAsync(CancellationToken cancellationToken = default)
        {
            var indexName = _indexName.Value;

            return _baseIndexTools.IsIndexExistsAsync(indexName, cancellationToken);
        }
    }
}