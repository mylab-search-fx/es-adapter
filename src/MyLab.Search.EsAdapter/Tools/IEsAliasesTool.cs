using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter.Inter;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities for working with aliases
    /// </summary>
    public interface IEsAliasesTool
    {
        /// <summary>
        /// Gets aliases
        /// </summary>
        Task<IEnumerable<IEsAliasTool>> GetAliasesAsync(Func<GetAliasDescriptor, IGetAliasRequest> selector = null, CancellationToken cancellationToken = default);
    }

    class EsAliasesTool : IEsAliasesTool
    {
        private readonly IEsClientProvider _clientProvider;
        private readonly IEsResponseValidator _responseValidator;

        public EsAliasesTool(IEsClientProvider clientProvider, IEsResponseValidator responseValidator)
        {
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
            _responseValidator = responseValidator;
        }

        public async Task<IEnumerable<IEsAliasTool>> GetAliasesAsync(Func<GetAliasDescriptor, IGetAliasRequest> selector = null, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.GetAliasAsync(null, selector, cancellationToken);

            if (resp.ApiCall.HttpStatusCode == 404)
                return Enumerable.Empty<IEsAliasTool>();

            _responseValidator.Validate(resp);

            return resp.Indices.SelectMany(idx =>
                idx.Value.Aliases.Select(a =>
                    new EsAliasTool(a.Key, idx.Key.Name, _clientProvider, _responseValidator)
                )
            );
        }
    }
}
