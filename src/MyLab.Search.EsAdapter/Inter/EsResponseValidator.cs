using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using MyLab.Log;
using Nest;

namespace MyLab.Search.EsAdapter.Inter
{
    /// <summary>
    /// Validates ES response
    /// </summary>
    public interface IEsResponseValidator
    {
        /// <summary>
        /// Validates a response and throws an exception if response is invalid
        /// </summary>
        void Validate(IResponse response, string errorMessage = null);

        /// <summary>
        /// Validates a response and throws an exception if response is invalid
        /// </summary>
        void Validate(ElasticsearchResponseBase response, string errorMessage = null);
    }

    /// <summary>
    /// Validates ES response
    /// </summary>
    public class EsResponseValidator : IEsResponseValidator
    {
        /// <summary>
        /// Get flag which determines response dumps body including
        /// </summary>
        public bool IncludeBodyInDump { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsResponseValidator"/>
        /// </summary>
        public EsResponseValidator(IOptions<EsOptions> opts)
            :this(opts.Value.IncludeDumpBody)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsResponseValidator"/>
        /// </summary>
        public EsResponseValidator(bool includeBodyInDump)
        {
            IncludeBodyInDump = includeBodyInDump;
        }

        /// <inheritdoc />
        public void Validate(IResponse response, string errorMessage = null)
        {
            if (response.ApiCall.Success) return;

            var ex = new EsException(errorMessage ?? "Elasticsearch interaction error", EsResponseDescription.FromResponse(response));
            if (response.ServerError != null)
                ex = ex.AndFactIs("server-error", response.ServerError);
            if (response.ApiCall != null)
                ex = ex.AndFactIs("dump", ApiCallDumper.ApiCallToDump(response.ApiCall, includeBody: IncludeBodyInDump));

            throw ex;
        }

        /// <inheritdoc />
        public void Validate(ElasticsearchResponseBase response, string errorMessage = null)
        {
            if (response.Success) return;

            var ex = new EsException(errorMessage ?? "Elasticsearch interaction error", EsResponseDescription.FromResponse(response));
            if (response.ApiCall != null)
                ex = ex.AndFactIs("dump", ApiCallDumper.ApiCallToDump(response.ApiCall, includeBody: IncludeBodyInDump));

            throw ex;
        }
    }
}