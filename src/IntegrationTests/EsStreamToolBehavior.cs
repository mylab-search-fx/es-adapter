using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;
using static IntegrationTests.EsStreamToolBehavior;

namespace IntegrationTests
{
    public class EsStreamToolBehavior : IClassFixture<TestClientFixture>, IClassFixture<IndexTemplateFixture>
    {
        private readonly IEsStreamTool _strmTool;
        private readonly SingleEsClientProvider _esClientProvider;

        public EsStreamToolBehavior(
            TestClientFixture clientFxt,
            IndexTemplateFixture indexTemplateFxt,
            ITestOutputHelper output)
        {
            clientFxt.Output = output;
            var client = clientFxt.Client;

            _esClientProvider = new SingleEsClientProvider(client);
            var streamName = indexTemplateFxt.NamePrefix + "-" + Guid.NewGuid().ToString("N");
            
            _strmTool = new EsStreamTool(streamName, _esClientProvider);

            output.WriteLine("Index template: " + indexTemplateFxt.TemplateName);
            output.WriteLine("Name prefix: " + indexTemplateFxt.NamePrefix);
        }

        [Fact]
        public async Task ShouldCreateStream()
        {
            //Arrange
            
            //Act
            await using var deleter = await _strmTool.CreateAsync();
            var streamExists = await _strmTool.ExistsAsync();

            //Assert
            Assert.True(streamExists);
        }

        [Fact]
        public async Task ShouldDeleteStream()
        {
            //Arrange
            await _strmTool.CreateAsync();

            //Act
            await _strmTool.DeleteAsync();

            var streamExists = await _strmTool.ExistsAsync();

            //Assert
            Assert.False(streamExists);
        }

        [Fact]
        public async Task ShouldNotUpdateExistentStream()
        {
            //Arrange
            await _strmTool.CreateAsync();

            //Act & Assert
            await Assert.ThrowsAsync<EsException>(() => _strmTool.CreateAsync());
        }

        [Fact]
        public async Task ShouldNotFindAbsentIndex()
        {
            //Arrange
            string randomStreamName = Guid.NewGuid().ToString("N");

            //Act
            var exists = await new EsStreamTool(randomStreamName, _esClientProvider).ExistsAsync();

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
