namespace MyLab.Elastic
{
    /// <summary>
    /// Provides index names
    /// </summary>
    public interface IIndexNameProvider
    {
        /// <summary>
        /// Provides index name for model type
        /// </summary>
        string Provide<TDoc>();
    }
}
