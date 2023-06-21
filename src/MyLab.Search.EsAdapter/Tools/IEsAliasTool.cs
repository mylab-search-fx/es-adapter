using MyLab.Search.EsAdapter.Inter;
using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Log;
using Nest;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Provides abilities to work with index alias
    /// </summary>
    public interface IEsAliasTool
    {
        /// <summary>
        /// Gets target name
        /// </summary>
        public string TargetName { get; }
        /// <summary>
        /// Gets alias name
        /// </summary>
        public string AliasName { get; }

        /// <summary>
        /// Gets true if alias exists
        /// </summary>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes an alias
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets alias definition
        /// </summary>
        Task<AliasDefinition> GetAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Create or update alias
        /// </summary>
        Task<IAsyncDisposable> PutAsync(Func<PutAliasDescriptor, IPutAliasRequest> selector = null, CancellationToken cancellationToken = default);
    }

    class EsAliasTool : IEsAliasTool
    {
        private readonly IEsClientProvider _clientProvider;

        public string TargetName { get; }
        public string AliasName { get; }

        public EsAliasTool(string aliasName, string targetName, IEsClientProvider clientProvider)
        {
            TargetName = targetName ?? throw new ArgumentNullException(nameof(targetName));
            AliasName = aliasName ?? throw new ArgumentNullException(nameof(aliasName));
            _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices
                .GetAliasAsync(TargetName, d => d.Name(AliasName), cancellationToken);

            if (!resp.IsValid && resp.ServerError.Status == 404)
                return false;

            EsException.ThrowIfInvalid(resp);

            return resp.Indices.TryGetValue(TargetName, out var aliases) && aliases.Aliases.ContainsKey(AliasName);
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices
                .DeleteAliasAsync(new DeleteAliasRequest(TargetName, AliasName), cancellationToken);
            
            EsException.ThrowIfInvalid(resp);
        }

        public async Task<AliasDefinition> GetAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices
                .GetAliasAsync(TargetName, d => d.Name(AliasName), cancellationToken);

            EsException.ThrowIfInvalid(resp);

            if(!resp.Indices.TryGetValue(TargetName, out var index))
                throw new InvalidOperationException("Index not found")
                    .AndFactIs("index", TargetName);

            if (!index.Aliases.TryGetValue(AliasName, out var alias))
                throw new InvalidOperationException("Alias not found")
                    .AndFactIs("index", TargetName)
                    .AndFactIs("alias", AliasName);

            return alias;
        }

        public async Task<IAsyncDisposable> PutAsync(Func<PutAliasDescriptor, IPutAliasRequest> selector = null, CancellationToken cancellationToken = default)
        {
            var resp = await _clientProvider.Provide().Indices.PutAliasAsync(TargetName, AliasName, selector, cancellationToken);

            EsException.ThrowIfInvalid(resp);

            return new AliasDeleter(this);
        }

        class AliasDeleter : IAsyncDisposable
        {
            private readonly IEsAliasTool _tool;

            public AliasDeleter(IEsAliasTool tool)
            {
                _tool = tool;
            }

            public async ValueTask DisposeAsync()
            {
                await _tool.DeleteAsync();
            }
        }
    }
}
