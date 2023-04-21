using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;
using Xunit.Abstractions;
using static IntegrationTests.EsStreamToolsBehavior;

namespace IntegrationTests
{
    public class EsStreamToolsBehavior : IClassFixture<TestClientFixture>, IClassFixture<IndexTemplateFixture>
    {
        private readonly IEsSpecialStreamTools _specStrmTools;
        private readonly IEsStreamTools _strmTools;

        public EsStreamToolsBehavior(
            TestClientFixture clientFxt,
            IndexTemplateFixture indexTemplateFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            var esClientProvider = new SingleEsClientProvider(client);
            var streamName = indexTemplateFxt.NamePrefix + "-" + Guid.NewGuid().ToString("N");

            _strmTools = new EsStreamTools(esClientProvider);
            _specStrmTools = new EsSpecialStreamTools(_strmTools, streamName);

            output.WriteLine("Index template: " + indexTemplateFxt.TemplateName);
            output.WriteLine("Name prefix: " + indexTemplateFxt.NamePrefix);
        }

        [Fact]
        public async Task ShouldCreateStream()
        {
            //Arrange
            
            //Act
            await using var deleter = await _specStrmTools.CreateStreamAsync();
            var streamExists = await _specStrmTools.IsStreamExistsAsync();

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldDeleteStream()
        {
            //Arrange
            await _specStrmTools.CreateStreamAsync();

            //Act
            await _specStrmTools.DeleteStreamAsync();

            var streamExists = await _specStrmTools.IsStreamExistsAsync();

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange
            string randomStreamName = Guid.NewGuid().ToString("N");

            //Act
            var exists = await _strmTools.IsStreamExistsAsync(randomStreamName);

            //Assert
            Assert.False(exists);
        }

        public class IndexTemplateFixture : IAsyncLifetime
        {
            private readonly ElasticClient _client;

            public string TemplateName { get; }
            public string NamePrefix { get; }

            public IndexTemplateFixture()
            {
                var settings = new ConnectionSettings(TestTools.ConnectionPool);
                settings.DisableDirectStreaming();

                _client = new ElasticClient(settings);

                TemplateName = Guid.NewGuid().ToString("N");
                NamePrefix = Guid.NewGuid().ToString("N");
            }

            public Task InitializeAsync()
            {
                return _client.LowLevel.DoRequestAsync<StringResponse>(
                    HttpMethod.PUT, 
                    $"_index_template/{TemplateName}", 
                    CancellationToken.None, 
                    $"{{\"index_patterns\": [\"{NamePrefix}*\"], \"data_stream\": {{ }}}}");
            }

            public Task DisposeAsync()
            {
                return _client.LowLevel.DoRequestAsync<StringResponse>(HttpMethod.DELETE, $"_index_template/{TemplateName}", CancellationToken.None);
            }
        }
    }
}
