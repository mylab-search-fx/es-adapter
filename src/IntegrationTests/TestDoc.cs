using System;
using MyLab.Search.EsAdapter;
using Nest;

namespace IntegrationTests
{
    [EsBindingKey("foo")]
    [ElasticsearchType(IdProperty = nameof(Id))]
    class TestDoc
    {
        [Keyword(Name = "id")] public string Id { get; set; }
        [Text(Name = "content")] public string Content { get; set; }
        [Text(Name = "content2")] public string Content2 { get; set; }

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
}