using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsStreamsToolBehavior : IClassFixture<TestClientFixture>, IAsyncLifetime
    {
        private readonly EsStreamsTool _streamsTool;
        private readonly IEsIndexTemplateTool _indexTemplate;
        private readonly IEsStreamTool _testStream;
        private readonly string _testStreamName;

        public EsStreamsToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var esClientProvider = new SingleEsClientProvider(fxt.Client);
            _streamsTool = new EsStreamsTool(esClientProvider);

            _testStreamName = Guid.NewGuid().ToString("N");
            _testStream = new EsStreamTool(_testStreamName, esClientProvider);

            _indexTemplate = new EsIndexTemplateTool(Guid.NewGuid().ToString("N"), esClientProvider);
        }

        public Task InitializeAsync()
        {
            return _indexTemplate.PutAsync(d => d.IndexPatterns(_testStreamName + "*"));
        }

        public async Task DisposeAsync()
        {
            bool exists = await _indexTemplate.ExistsAsync();
            if (exists)
            {
                await _indexTemplate.DeleteAsync();
            }
        }

        [Fact]
        public async Task ShouldCreateStream()
        {
            //Arrange
            ICreateDataStreamRequest newStreamReq = new CreateDataStreamRequest(_testStreamName);

            //Act
            await _streamsTool.CreateAsync(newStreamReq);

            bool exists = await _testStream.ExistsAsync();

            if (exists)
            {
                await _testStream.DeleteAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldEnumerateStreams()
        {
            //Arrange
            await _testStream.CreateAsync();

            //Act
            var streams = await _streamsTool.GetAsync();
            var streamsArray = streams.ToArray();

            //Assert
            Assert.Contains(streamsArray, a => a.Name == _testStreamName);
        }

        [Fact]
        public async Task ShouldFilterIndexes()
        {
            //Arrange
            await _testStream.CreateAsync();

            //Act
            var streams = await _streamsTool.GetAsync(d => d.Name(_testStreamName));
            var streamArray = streams.ToArray();

            //Assert
            Assert.Contains(streamArray, a => a.Name == _testStreamName);
        }

        [Fact]
        public async Task ShouldDeleteStreams()
        {
            //Arrange
            await _testStream.CreateAsync();

            //Act
            await _streamsTool.GetAsync(d => d.Name(_testStreamName));

            var exists = await _testStream.ExistsAsync();

            //Assert
            Assert.False(exists);
        }
    }
}
