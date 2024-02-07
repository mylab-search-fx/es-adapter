using System;

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
    }
}
