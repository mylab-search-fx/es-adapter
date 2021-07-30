using Nest;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Highlight selector
    /// </summary>
    public delegate IHighlight EsHlSelector<TDoc>(HighlightDescriptor<TDoc> descriptor)
        where TDoc : class;
}