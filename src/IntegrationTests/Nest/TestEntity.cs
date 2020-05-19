using Nest;

namespace IntegrationTests.Nest
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    class TestEntity
    {
        [Number(NumberType.Integer, Name = "uid")]
        public int Id { get; set; }
        [Text(Name = "val")]
        public string Value { get; set; }
    }
}