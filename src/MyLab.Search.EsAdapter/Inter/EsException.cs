using System;
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
        /// Initializes a new instance of <see cref="EsException"/>
        /// </summary>
        public EsException(string message, IResponse response)
            : base(message, response.OriginalException)
        {
            
        }

        /// <summary>
        /// Checks the response and throws an exception if response is invalid
        /// </summary>
        public static void ThrowIfInvalid(IResponse response, string errorMessage = null)
        {
            if(response.IsValid) return;

            var ex = new EsException(errorMessage ?? "Elasticsearch interaction error", response);
            if (response.DebugInformation != null)
                ex = ex.AndFactIs("debug-info", response.DebugInformation);
            if(response.ServerError != null)
                ex = ex.AndFactIs("server-error", response.ServerError);
            if(response.ApiCall != null)
                ex = ex.AndFactIs("dump", ApiCallDumper.ApiCallToDump(response.ApiCall));

            throw ex;
        }
    }
}
