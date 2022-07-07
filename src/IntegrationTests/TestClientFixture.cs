using MyLab.Search.EsAdapter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class TestClientFixture
    {
        public ElasticClient GetClientProvider(ITestOutputHelper output)
        {
            var settings = new ConnectionSettings(TestTools.ConnectionPool);

            settings.DisableDirectStreaming();
            settings.OnRequestCompleted(details =>
            {
                output?.WriteLine(ApiCallDumper.ApiCallToDump(details));
            });

            return new ElasticClient(settings);
        }
    }
}