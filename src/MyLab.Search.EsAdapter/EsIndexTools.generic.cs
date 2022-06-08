using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter
{
    public class EsIndexTools<TDoc> : IEsIndexTools<TDoc> where TDoc: class
    {
        private readonly IEsIndexTools _baseIndexTools;
        private readonly string _indexName;

        public EsIndexTools(IEsIndexTools baseIndexTools, IOptions<EsOptions> options)
            :this(baseIndexTools, options.Value)
        {
            
        }

        public EsIndexTools(IEsIndexTools baseIndexTools, EsOptions options)
            : this(baseIndexTools, new OptionsIndexNameProvider(options))
        {

        }

        public EsIndexTools(IEsIndexTools baseIndexTools, IIndexNameProvider indexNameProvider)
        {
            _baseIndexTools = baseIndexTools;
            _indexName = indexNameProvider.Provide<TDoc>();
        }

        public Task<IIndexDeleter> CreateIndexAsync(Func<CreateIndexDescriptor, ICreateIndexRequest> createDescriptor = null, CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.CreateIndexAsync(_indexName, createDescriptor, cancellationToken);
        }

        public Task<IIndexDeleter> CreateIndexAsync(string jsonSettings, CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.CreateIndexAsync(_indexName, jsonSettings, cancellationToken);
        }

        public Task DeleteIndexAsync(CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.DeleteIndexAsync(_indexName, cancellationToken);
        }

        public Task<bool> IsIndexExistsAsync(CancellationToken cancellationToken = default)
        {
            return _baseIndexTools.IsIndexExistsAsync(_indexName, cancellationToken);
        }
    }
}