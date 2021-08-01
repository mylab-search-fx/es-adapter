using System;
using Elasticsearch.Net;
using MyLab.Log;
using Nest;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Extensions for responses
    /// </summary>
    public static class EsResponseExtensions
    {
        /// <summary>
        /// Throws <see cref="ResponseException"/> if response is not valid
        /// </summary>
        public static void ThrowIfInvalid(this IResponse response, string msg)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (!response.IsValid)
                throw new ResponseException(msg, response)
                    .AndFactIs(response.ApiCall)
                    .AndFactIs("server-error", response.ServerError?.ToString() ?? "[null]"); 
        }

        /// <summary>
        /// Throws <see cref="ResponseException"/> if response is not valid
        /// </summary>
        public static void ThrowIfInvalid(this IElasticsearchResponse response, string msg)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (!response.ApiCall.Success)
                throw new LowLevelResponseException(msg, response)
                    .AndFactIs(response.ApiCall);
        }
    }
}
