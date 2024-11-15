using System;
using MyLab.Search.EsAdapter;
using Nest;

namespace IntegrationTests
{
    [EsBindingKey("foo")]
    [ElasticsearchType(IdProperty = nameof(Id))]
    class TestDoc
    {
        public const string IdName = "id";
        public const string ContentName = "content_1";
        public const string Content2Name = "Content_2";

        [Keyword(Name = IdName)] public string Id { get; set; }
        [Text(Name = ContentName)] public string Content { get; set; }
        [Text(Name = Content2Name)] public string Content2 { get; set; }

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