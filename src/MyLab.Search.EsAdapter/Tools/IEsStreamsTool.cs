﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with streams
    /// </summary>
    public interface IEsStreamsTool
    {
        /// <summary>
        /// Gets streams with selector
        /// </summary>
        Task<IEnumerable<FoundStream>> GetAsync(
            Func<GetDataStreamDescriptor, IGetDataStreamRequest> selector = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes the streams by exactly names
        /// </summary>
        Task DeleteByExactlyNamesAsync(
            string[] exactlyNames,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes the stream by name or several streams by wildcard expressions
        /// </summary>
        Task DeleteByNameOrWildcardExpressionAsync(
            string nameOrWildcardExpression,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Create stream with <see cref="ICreateDataStreamRequest"/>
        /// </summary>
        Task<IEsStreamTool> CreateAsync(
            ICreateDataStreamRequest createRequest,
            CancellationToken cancellationToken = default
        );
    }

    /// <summary>
    /// Represent a found stream
    /// </summary>
    public record FoundStream(string Name, DataStreamInfo Info, IEsStreamTool Tool);

    class EsStreamsTool : IEsStreamsTool
    {
        private readonly IEsClientProvider _clientProvider;
        private readonly IEsResponseValidator _responseValidator;

        public EsStreamsTool(IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
            _responseValidator = responseValidator;
        }
        public async Task<IEnumerable<FoundStream>> GetAsync(
                Func<GetDataStreamDescriptor, IGetDataStreamRequest> selector = null, 
                CancellationToken cancellationToken = default
            )
        {
            var response = await _clientProvider.Provide().Indices
                .GetDataStreamAsync(null, selector, cancellationToken);
            
            _responseValidator.Validate(response, "Unable to get streams");

            return response.DataStreams.Select(i =>
                new FoundStream(i.Name, i, new EsStreamTool(i.Name, _clientProvider, _responseValidator))
            );
        }

        public async Task DeleteByExactlyNamesAsync(
                string[] exactlyNames, 
                CancellationToken cancellationToken = default
            )
        {
            if (exactlyNames == null) throw new ArgumentNullException(nameof(exactlyNames));
            var oneName = string.Join(',', exactlyNames);

            var response = await _clientProvider.Provide().Indices.DeleteDataStreamAsync(oneName, _ => _, cancellationToken);

            _responseValidator.Validate(response, "Unable to delete streams");
        }

        public async Task DeleteByNameOrWildcardExpressionAsync(
            string nameOrWildcardExpression,
            CancellationToken cancellationToken = default
        )
        {
            if (nameOrWildcardExpression == null) throw new ArgumentNullException(nameof(nameOrWildcardExpression));
            
            var response = await _clientProvider.Provide().Indices.DeleteDataStreamAsync(nameOrWildcardExpression, _ => _, cancellationToken);

            _responseValidator.Validate(response, "Unable to delete streams");
        }

        public async Task<IEsStreamTool> CreateAsync(
                ICreateDataStreamRequest createRequest, 
                CancellationToken cancellationToken = default
            )
        {
            var resp = await _clientProvider.Provide().Indices
                .CreateDataStreamAsync(createRequest, cancellationToken);

            _responseValidator.Validate(resp, "Unable to create the stream");

            return new EsStreamTool(createRequest.Name.ToString(), _clientProvider, _responseValidator);
        }
    }
}
