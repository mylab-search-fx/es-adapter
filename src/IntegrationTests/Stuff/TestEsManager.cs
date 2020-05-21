using MyLab.Elastic;
using Nest;

namespace IntegrationTests.Stuff
{
    class TestEsManager : IEsManager
    {
        public ElasticClient Client { get; }

        public TestEsManager(ElasticClient client)
        {
            Client = client;
        }
    }
}
