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
        public static string ApiCallToDump(IApiCallDetails apiCall)
        {
            var call = apiCall;
            var sb = new StringBuilder();

            sb.AppendLine("# REQUEST");
            sb.AppendLine();
            sb.AppendLine($"{call.HttpMethod} {call.Uri}");
            sb.AppendLine();

            if (call.RequestBodyInBytes != null)
            {
                var formattedReqBody = DumpToString(call.RequestBodyInBytes);
                sb.AppendLine(formattedReqBody);
            }
            else
            {
                sb.AppendLine("[no request body]");
            }

            sb.AppendLine();
            sb.AppendLine("# RESPONSE");
            sb.AppendLine();

            if (call.HttpStatusCode.HasValue)
            {
                HttpStatusCode statusCode = (HttpStatusCode)call.HttpStatusCode;
                sb.AppendLine($"{(int)statusCode} ({statusCode})");
            }
            else
            {
                sb.AppendLine($"[null]");
            }

            sb.AppendLine();

            if (call.ResponseBodyInBytes != null)
            {
                var formattedRespBody = DumpToString(call.ResponseBodyInBytes);
                sb.AppendLine(formattedRespBody);
            }
            else
            {
                sb.AppendLine("[no response body]");
            }

            sb.AppendLine();
            sb.AppendLine("# END");

            return sb.ToString();
        }

        private static string DumpToString(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);

            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(str);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                return str;
            }
        }
    }
}