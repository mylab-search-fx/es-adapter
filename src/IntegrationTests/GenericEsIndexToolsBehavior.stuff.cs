using System;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class SpecialEsIndexToolsBehavior
    {
        private readonly ElasticClient _client;
        private readonly EsSpecialIndexTools<TestDoc> _specialIndexTools;
        private readonly string _indexName;

        public SpecialEsIndexToolsBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _client = fxt.Client;

            var esClientProvider = new SingleEsClientProvider(_client);
            var baseIndexTools = new EsIndexTools(esClientProvider);

            _indexName = Guid.NewGuid().ToString("N");

            _specialIndexTools = new EsSpecialIndexTools<TestDoc>(baseIndexTools, new SingleIndexNameProvider(_indexName));
        }
    }
}