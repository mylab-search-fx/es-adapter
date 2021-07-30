namespace MyLab.Search.EsAdapter
{
    public class SingleIndexNameProvider : IIndexNameProvider
    {
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of <see cref="SingleIndexNameProvider"/>
        /// </summary>
        public SingleIndexNameProvider(string indexName)
        {
            _indexName = indexName;
        }
        public string Provide<TDoc>()
        {
            return _indexName;
        }
    }
}