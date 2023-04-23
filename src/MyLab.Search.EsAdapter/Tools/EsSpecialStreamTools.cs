using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace MyLab.Search.EsAdapter.Tools
{
    /// <summary>
    /// Default implementation for <see cref="IEsSpecialStreamTools"/>
    /// </summary>
    public class EsSpecialStreamTools<TDoc> : EsSpecialStreamTools where TDoc : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsSpecialStreamTools(IEsStreamTools baseStreamTools, IOptions<EsOptions> options)
            : this(baseStreamTools, options.Value)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsSpecialStreamTools(IEsStreamTools baseStreamTools, EsOptions options)
            : this(baseStreamTools, new OptionsIndexNameProvider(options))
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsSpecialStreamTools(IEsStreamTools baseStreamTools, IIndexNameProvider indexNameProvider)
            : base(baseStreamTools, indexNameProvider.Provide<TDoc>())
        {
        }
    }

    /// <summary>
    /// Default implementation for <see cref="IEsSpecialStreamTools"/>
    /// </summary>
    public class EsSpecialStreamTools : IEsSpecialStreamTools
    {
        private readonly IEsStreamTools _baseStreamTools;
        private readonly string _streamName;

        /// <summary>
        /// Initializes a new instance of <see cref="EsIndexTools"/>
        /// </summary>
        public EsSpecialStreamTools(IEsStreamTools baseStreamTools, string streamName)
        {
            _baseStreamTools = baseStreamTools;
            _streamName = streamName;
        }

        /// <inheritdoc />
        public Task<IAsyncDisposable> CreateStreamAsync(CancellationToken cancellationToken = default)
        {
            return _baseStreamTools.CreateStreamAsync(_streamName, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteStreamAsync(CancellationToken cancellationToken = default)
        {
            return _baseStreamTools.DeleteStreamAsync(_streamName, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> IsStreamExistsAsync(CancellationToken cancellationToken = default)
        {
            return _baseStreamTools.IsStreamExistsAsync(_streamName, cancellationToken);
        }
    }
}