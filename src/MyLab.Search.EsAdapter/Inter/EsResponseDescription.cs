using System;
using System.Linq;
using Elasticsearch.Net;
using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Describe Elasticsearch response
    /// </summary>
    /// todo: To remove
    [Obsolete("Will be removed in next versions")]
    public class EsResponseDescription
    {
        /// <summary>
        /// A human readable string representation of what happened during this request for both successful and failed requests.
        /// </summary>
        public string DebugInformation { get; init; }

        /// <summary>
        /// The Elasticsearch.Net.IApiCallDetails diagnostic information
        /// </summary>
        public IApiCallDetails ApiCall { get; init; }

        /// <summary>
        /// If Elasticsearch.Net.IApiCallDetails.Success is false, this will hold the original exception. This will be the originating CLR exception in most cases.
        /// </summary>
        public Exception BaseException { get; init; }

        /// <summary>
        /// Gets error about the index absence 
        /// </summary>
        public bool HasIndexNotFound { get; private set; }

        /// <summary>
        /// Creates response description from response object
        /// </summary>
        public static EsResponseDescription FromResponse(ElasticsearchResponseBase response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return new EsResponseDescription
            {
                DebugInformation = response.DebugInformation,
                ApiCall = response.ApiCall,
                BaseException = response.OriginalException
            };
        }

        /// <summary>
        /// Creates response description from response object
        /// </summary>
        public static EsResponseDescription FromResponse(IResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            bool hasIndexNotFound;

            if (response is BulkResponse bulkResp)
            {
                hasIndexNotFound = bulkResp.ItemsWithErrors != null &&
                                   bulkResp.ItemsWithErrors.Any(itm => itm.Status == 404);
            }
            else
            {
                hasIndexNotFound = response.ServerError?.Error?.Type == "index_not_found_exception";
            }

            return new EsResponseDescription
            {
                DebugInformation = response.DebugInformation,
                ApiCall = response.ApiCall,
                HasIndexNotFound = hasIndexNotFound
            };
        }
    }
}