using System;
using Elasticsearch.Net;
using MyLab.Log;
using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Contains details about interaction error
    /// </summary>
    public class EsException : Exception
    {
        /// <summary>
        /// Elasticsearch response
        /// </summary>
        public EsResponseDescription Response { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsException"/>
        /// </summary>
        public EsException(string message, EsResponseDescription response)
            : base(message, response.BaseException)
        {
            Response = response;
        }

        /// <summary>
        /// Checks the response and throws an exception if response is invalid
        /// </summary>
        public static void ThrowIfInvalid(IResponse response, string errorMessage = null)
        {
            if(response.IsValid) return;

            var ex = new EsException(errorMessage ?? "Elasticsearch interaction error", EsResponseDescription.FromResponse(response));
            if(response.ServerError != null)
                ex = ex.AndFactIs("server-error", response.ServerError);
            if (response.ApiCall != null)
                ex = ex.AndFactIs("dump", ApiCallDumper.ApiCallToDump(response.ApiCall, includeBody: false));

            throw ex;
        }

        /// <summary>
        /// Checks the response and throws an exception if response is invalid
        /// </summary>
        public static void ThrowIfInvalid(ElasticsearchResponseBase response, string errorMessage = null)
        {
            if (response.Success) return;

            var ex = new EsException(errorMessage ?? "Elasticsearch interaction error", EsResponseDescription.FromResponse(response));
            if (response.ApiCall != null)
                ex = ex.AndFactIs("dump", ApiCallDumper.ApiCallToDump(response.ApiCall, includeBody: false));

            throw ex;
        }
    }
}
