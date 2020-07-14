using System;
using Elasticsearch.Net;
using MyLab.LogDsl;
using MyLab.Logging;

namespace MyLab.Elastic
{
    /// <summary>
    /// Provides extension methods for logging
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Adds fact about ES call into log
        /// </summary>
        public static DslLogEntityBuilder AndFactIs(this DslLogEntityBuilder logger, IApiCallDetails apiCallDetails)
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
