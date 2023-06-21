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

        public EsAliasesTool(IEsClientProvider clientProvider)
        {
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        }

        public async Task<IEnumerable<IEsAliasTool>> GetAliasesAsync(Func<GetAliasDescriptor, IGetAliasRequest> selector = null, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.GetAliasAsync(null, selector, cancellationToken);

            EsException.ThrowIfInvalid(resp);

            return resp.Indices.SelectMany(idx =>
                idx.Value.Aliases.Select(a =>
                    new EsAliasTool(a.Key, idx.Key.Name, _clientProvider)
                )
            );
        }
    }
}
