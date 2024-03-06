using System.Net;
using System.Text;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Creates string dump from <see cref="IApiCallDetails"/>
    /// </summary>
    public static class ApiCallDumper
    {
        /// <summary>
        /// Creates string dump from <see cref="IApiCallDetails"/>
        /// </summary>
        public static string ApiCallToDump(IApiCallDetails apiCall, bool includeBody = true)
        {
            var call = apiCall;
            var sb = new StringBuilder();
            
            sb.Append("# REQUEST\n");
            sb.Append("\n");
            sb.Append($"{call.HttpMethod} {call.Uri}\n");
            sb.Append("\n");

            if (call.RequestBodyInBytes != null)
            {
                var formattedReqBody = includeBody 
                    ? DumpToString(call.RequestBodyInBytes, "application/json")
                    : "[body was excluded]";
                sb.Append(formattedReqBody + "\n");
            }
            else
            {
                sb.Append("[no request body]\n");
            }

            sb.Append("\n");
            sb.Append("# RESPONSE\n");
            sb.Append("\n");

            if (call.HttpStatusCode.HasValue)
            {
                HttpStatusCode statusCode = (HttpStatusCode)call.HttpStatusCode;
                sb.Append($"{(int)statusCode} ({statusCode})\n");
            }
            else
            {
                sb.Append("[null]\n");
            }

            sb.Append("\n");

            if (call.ResponseBodyInBytes != null)
            {
                var formattedRespBody = includeBody 
                    ? DumpToString(call.ResponseBodyInBytes, call.ResponseMimeType)
                    : "[body was excluded]";
                sb.Append(formattedRespBody+ "\n");
            }
            else
            {
                sb.Append("[no response body]\n");
            }

            sb.Append("\n");
            sb.Append("# END\n");

            return sb.ToString();
        }

        private static string DumpToString(byte[] data, string mimeType)
        {
            var str = Encoding.UTF8.GetString(data);

            if (mimeType != "application/json")
                return str;

            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(str);
                str = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

                return str.Replace("\r\n", "\n");
            }
            catch (JsonReaderException)
            {
                return str;
            }
            catch (JsonSerializationException)
            {
                return str;
            }
        }
    }
}