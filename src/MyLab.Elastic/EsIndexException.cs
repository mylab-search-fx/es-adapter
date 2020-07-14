using System;
using Elasticsearch.Net;
using MyLab.Logging;
using Nest;

namespace MyLab.Elastic
{
    public class EsIndexManyException : ResponseException
    {
        public EsIndexManyException(BulkResponse resp) : base("Can't index documents", resp)
        {
        }
    }

    public class EsIndexException : ResponseException
    {
        public EsIndexException(IndexResponse resp): base("Can't index document", resp)
        {
        }
    }

    public class EsSearchException<TDoc> : ResponseException
        where TDoc : class
    {
        public EsSearchException(ISearchResponse<TDoc> resp) : base("Can't perform search", resp)
        {
        }
    }

    public class ResponseException : ElasticsearchException
    {
        public IResponse Response { get; }

        public ResponseException(string msg, IResponse resp) 
            : base(msg, resp.OriginalException)
        {
            Response = resp;
        }
    }

    public class LowLevelResponseException : ElasticsearchException
    {
        public IElasticsearchResponse Response { get; }

        public LowLevelResponseException(string msg, IElasticsearchResponse resp)
            : base(msg)
        {
            Response = resp;
        }
    }

    public class ElasticsearchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchException"/>
        /// </summary>
        public ElasticsearchException(string message)
            :base(message)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchException"/>
        /// </summary>
        public ElasticsearchException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}