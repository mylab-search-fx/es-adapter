using System;

namespace MyLab.Elastic
{
    /// <summary>
    /// Determines constant binding to elastic search index
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ElasticsearchIndexAttribute : Attribute
    {
        public string IndexName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchIndexAttribute"/>
        /// </summary>
        public ElasticsearchIndexAttribute(string indexName)
        {
            IndexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
        }
    }
}
