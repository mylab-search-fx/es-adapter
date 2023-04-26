using System;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class SpecialEsIndexToolsBehavior
    {
        private readonly ElasticClient _client;
        private readonly IEsIndexTool _indexTool;

        public SpecialEsIndexToolsBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _client = fxt.Client;

            var esClientProvider = new SingleEsClientProvider(_client);

            var indexName = Guid.NewGuid().ToString("N");
            _indexTool = new EsIndexTool(indexName, esClientProvider);
        }
    }
}