using System;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class GenericEsIndexToolsBehavior
    {
        private readonly ElasticClient _client;
        private readonly EsIndexTools<TestDoc> _indexTools;
        private readonly string _indexName;

        public GenericEsIndexToolsBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            _client = fxt.GetClientProvider(output);
            var esClientProvider = new SingleEsClientProvider(_client);
            var baseIndexTools = new EsIndexTools(esClientProvider);

            _indexName = Guid.NewGuid().ToString("N");

            var options = new EsOptions
            {
                IndexBindings = new[]
                {
                    new IndexBinding
                    {
                        Doc = "foo",
                        Index = _indexName
                    }
                }
            };

            _indexTools = new EsIndexTools<TestDoc>(baseIndexTools, options);
        }

        [EsBindingKey("foo")]
        private class TestDoc
        {
        }
    }
}