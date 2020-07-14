using System;
using MyLab.Logging;
using Nest;

namespace MyLab.Elastic
{
    public class EsIndexManyException : ResponseException<BulkResponse>
    {
        public EsIndexManyException(BulkResponse resp) : base("Can't index documents", resp)
        {
        }
    }

    public class EsIndexException : ResponseException<IndexResponse>
    {
        public EsIndexException(IndexResponse resp): base("Can't index document", resp)
        {
        }
    }

    public class EsSearchException<TDoc> : ResponseException<ISearchResponse<TDoc>> 
        where TDoc : class
    {
        public EsSearchException(ISearchResponse<TDoc> resp) : base("Can't perform search", resp)
        {
        }
    }

    public class ResponseException<TResp> : ElasticsearchException
        where TResp : IResponse
    {
        public TResp Response { get; }

        public ResponseException(string msg, TResp resp) 
            : base(msg, resp.OriginalException)
        {
            Response = resp;
            this.AndFactIs(resp.ApiCall);
            this.AndFactIs("server-error", resp.ServerError.ToString());
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