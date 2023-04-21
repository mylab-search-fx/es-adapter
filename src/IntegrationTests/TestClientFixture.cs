using MyLab.Search.EsAdapter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class TestClientFixture
    {
        public ITestOutputHelper Output { get; set; }

        public ElasticClient Client { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestClientFixture"/>
        /// </summary>
        public TestClientFixture()
        {
            var settings = new ConnectionSettings(TestTools.ConnectionPool);

            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                Output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });

            Client = new ElasticClient(settings);
        }
    }
}