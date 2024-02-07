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
            _streamsTool = new EsStreamsTool(esClientProvider, TestTools.ResponseValidator);

            _testStreamName = Guid.NewGuid().ToString("N");
            _testStream = new EsStreamTool(_testStreamName, esClientProvider, TestTools.ResponseValidator);

            _indexTemplate = new EsIndexTemplateTool(Guid.NewGuid().ToString("N"), esClientProvider, TestTools.ResponseValidator);
        }

        public Task InitializeAsync()
        {
            return _indexTemplate.PutAsync(
                d => d
                    .IndexPatterns(_testStreamName + "*")
                    .DataStream(new DataStream())
                );
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
            await using var streamDisposer = await _testStream.CreateAsync();

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
            await using var streamDisposer = await _testStream.CreateAsync();

            //Act
            var streams = await _streamsTool.GetAsync(d => d.Name(_testStreamName));
            var streamArray = streams.ToArray();

            //Assert
            Assert.Contains(streamArray, a => a.Name == _testStreamName);
        }

        [Fact]
        public async Task ShouldDeleteStreamsByExactlyNames()
        {
            //Arrange
            await _testStream.CreateAsync();

            //Act
            await _streamsTool.DeleteByExactlyNamesAsync(new string[]{ _testStreamName });

            var exists = await _testStream.ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldDeleteStreamsByName()
        {
            //Arrange
            await _testStream.CreateAsync();

            //Act
            await _streamsTool.DeleteByNameOrWildcardExpressionAsync(_testStreamName);

            var exists = await _testStream.ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldDeleteStreamsByWildcardExpression()
        {
            //Arrange
            await _testStream.CreateAsync();
            var wildcardExpression = _testStreamName.Remove(_testStreamName.Length - 1) + "*";

            //Act
            await _streamsTool.DeleteByNameOrWildcardExpressionAsync(wildcardExpression);

            var exists = await _testStream.ExistsAsync();

            //Assert
            Assert.False(exists);
        }
    }
}
