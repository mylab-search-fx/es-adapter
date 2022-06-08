﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class EsIndexerBehavior
    {
        private readonly ElasticClient _client;
        private readonly EsIndexer _indexer;
        private readonly string _indexName;
        private readonly EsIndexTools _indexTools;
        private IIndexDeleter _indexDeleter;

        public EsIndexerBehavior(TestClientFixture fxt, ITestOutputHelper output)
        {
            _client = fxt.GetClientProvider(output);
            var esClientProvider = new SingleEsClientProvider(_client);

            _indexName = Guid.NewGuid().ToString("N");

            _indexer = new EsIndexer(esClientProvider);
            _indexTools = new EsIndexTools(esClientProvider);
        }

        private async Task<TestDoc> SearchAsync(string id)
        {
            var res = await _client.SearchAsync(GetSearchFunc());

            EsException.ThrowIfInvalid(res);

            return res.Hits.FirstOrDefault()?.Source;

            Func<SearchDescriptor<TestDoc>, ISearchRequest> GetSearchFunc()
            {
                return sd =>
                    sd.Index(_indexName).Query(qd => qd.Ids(ids => ids.Values(id)));
            }
        }

        [ElasticsearchType(IdProperty = nameof(Id))]
        private class TestDoc
        {
            [Keyword(Name = "id")] public string Id { get; set; }
            [Keyword(Name = "content")] public string Content { get; set; }
            [Keyword(Name = "content2")] public string Content2 { get; set; }

            public static TestDoc Generate(string id = null)
            {
                return new TestDoc
                {
                    Id = id ?? Guid.NewGuid().ToString("N"),
                    Content = Guid.NewGuid().ToString("N"),
                    Content2 = Guid.NewGuid().ToString("N"),
                };
            }
        }

        public async Task InitializeAsync()
        {
            _indexDeleter = await _indexTools.CreateIndexAsync(_indexName);
        }

        public async Task DisposeAsync()
        {
            if (_indexDeleter != null)
                await _indexDeleter.DisposeAsync();
        }
    }
}