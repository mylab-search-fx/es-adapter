using Nest;

namespace MyLab.Search.EsAdapter.Search
{
    /// <summary>
    /// Highlight selector
    /// </summary>
    public delegate IHighlight EsHlSelector<TDoc>(HighlightDescriptor<TDoc> descriptor)
        where TDoc : class;
}