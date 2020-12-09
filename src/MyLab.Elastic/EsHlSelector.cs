using Nest;

namespace MyLab.Elastic
{
    /// <summary>
    /// Highlight selector
    /// </summary>
    public delegate IHighlight EsHlSelector<TDoc>(HighlightDescriptor<TDoc> descriptor)
        where TDoc : class;
}