using MyLab.Search.EsAdapter.Inter;
using MyLab.Search.EsAdapter.Tools;
using System;
using System.Threading.Tasks;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EsStreamAliasToolBehavior : IClassFixture<TestClientFixture>, IAsyncLifetime
    {
        private readonly IEsStreamTool _streamTool;
        private readonly IEsIndexTemplateTool _indexTemplateTool;
        private readonly Func<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request> _idxTemplateRequestFactory;
        private readonly SingleEsClientProvider _esClientProvider;

        public EsStreamAliasToolBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            var client = fxt.Client;
            _esClientProvider = new SingleEsClientProvider(client);

            var streamName = Guid.NewGuid().ToString("N");
            _streamTool = new EsStreamTool(streamName, _esClientProvider, TestTools.ResponseValidator);

            var templateName = Guid.NewGuid().ToString("N");
            _indexTemplateTool = new EsIndexTemplateTool(templateName, _esClientProvider, TestTools.ResponseValidator);

            _idxTemplateRequestFactory = d => d
                .IndexPatterns(new[] { streamName })
                .DataStream(new DataStream())
                .Template(td => td
                    .Aliases(asd => asd
                        .Alias("bar", ad => ad)));
        }

        public async Task InitializeAsync()
        {
            await DeleteComponentsAsync();

            var client = _esClientProvider.Provide();

            var resp = await client.Indices
                    .GetAliasAsync(null, d => d.Name(new Names(new[] { "foo", "bar" })));

            foreach (var foundIndex in resp.Indices)
            {
                foreach (var indexAlias in foundIndex.Value.Aliases)
                {
                    await client.Indices.DeleteAliasAsync(foundIndex.Key, indexAlias.Key);
                }
            }

            await _indexTemplateTool.PutAsync(_idxTemplateRequestFactory);
            await _streamTool.CreateAsync();
        }

        public Task DisposeAsync()
        {
            return DeleteComponentsAsync();
        }

        async Task DeleteComponentsAsync()
        {
            bool streamExists = await _streamTool.ExistsAsync();
            if (streamExists)
                await _streamTool.DeleteAsync();
            bool idxTemplateExists = await _indexTemplateTool.ExistsAsync();
            if (idxTemplateExists)
                await _indexTemplateTool.DeleteAsync();
        }

        [Fact]
        public async Task ShouldDetectExistentAlias()
        {
            //Arrange
            var tool = _streamTool.Alias("bar");

            //Act
            var exists = await tool.ExistsAsync();

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldNotDetectAbsentAlias()
        {
            //Arrange
            var tool = _streamTool.Alias("absent");

            //Act
            var exists = await tool.ExistsAsync();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldGetAlias()
        {
            //Arrange
            await using var aliasDeleter = await _streamTool.Alias("foo").PutAsync();

            var tool = _streamTool.Alias("foo");

            //Act
            var aliasDef = await tool.GetAsync();

            //Assert
            Assert.NotNull(aliasDef);
        }

        [Fact]
        public async Task ShouldDeleteAlias()
        {
            //Arrange
            var aliasDeleter = await _streamTool.Alias("foo").PutAsync();

            bool aliasExists = false;
            try
            {
                var tool = _streamTool.Alias("foo");
                await tool.DeleteAsync();

                //Act
                aliasExists = await tool.ExistsAsync();

                if (aliasExists)
                    await aliasDeleter.DisposeAsync();
            }
            catch (Exception)
            {
                await aliasDeleter.DisposeAsync();
            }

            //Assert
            Assert.False(aliasExists);
        }

        [Fact]
        public async Task ShouldCreateAlias()
        {
            //Arrange
            await using var aliasDeleter = await _streamTool.Alias("foo").PutAsync();

            var tool = _streamTool.Alias("foo");

            //Act
            var exists = await tool.ExistsAsync();

            //Assert
            Assert.True(exists);
        }
    }
}
