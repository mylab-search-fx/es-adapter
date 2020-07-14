using System;
using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Extensions for <see cref="IResponse"/>
    /// </summary>
    public static class EsResponseExtensions
    {
        /// <summary>
        /// Throws <see cref="ResponseException{TResp}"/> if response is not valid
        /// </summary>
        public static void ThrowIfInvalid<TResp>(this TResp response, string msg)
            where TResp : IResponse
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (response.IsValid)
                throw new ResponseException<TResp>(msg, response);
        }
    }
}
