using System;
using Elasticsearch.Net;
using MyLab.Log.Dsl;
using MyLab.Log;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Provides extension methods for logging
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Adds fact about ES call into log
        /// </summary>
        public static DslExpression AndFactIs(this DslExpression logger, IApiCallDetails apiCallDetails)
        {
            return logger.AndFactIs("es-call-details", 
                apiCallDetails != null 
                    ? ApiCallDumper.ApiCallToDump(apiCallDetails)
                    : "[null]");
        }

        /// <summary>
        /// Adds fact about ES call into exception
        /// </summary>
        public static Exception AndFactIs(this Exception exception, IApiCallDetails apiCallDetails)
        {
            return exception.AndFactIs("es-call-details",
                apiCallDetails != null
                    ? ApiCallDumper.ApiCallToDump(apiCallDetails)
                    : "[null]");
        }
    }
}
