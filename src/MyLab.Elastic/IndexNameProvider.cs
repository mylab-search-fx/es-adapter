using System;
using Microsoft.Extensions.Options;

namespace MyLab.Elastic
{
    class IndexNameProvider : IIndexNameProvider
    {
        private readonly string _defaultIndex;

        public IndexNameProvider(IOptions<ElasticsearchOptions> options)
            :this(options.Value)
        {

        }

        public IndexNameProvider(ElasticsearchOptions options)
        {
            _defaultIndex = options.DefaultIndex;
        }

        public string Provide<TDoc>()
        {
            if(_defaultIndex == null)
                throw new InvalidOperationException($"Index not fund for type '{typeof(TDoc).FullName}'");

            return _defaultIndex;
        }
    }
}