namespace MyLab.Elastic
{
    public class HighLightedDocument<TDoc>
    {
        /// <summary>
        /// Found documents
        /// </summary>
        public TDoc Doc { get; }

        /// <summary>
        /// Highlight string
        /// </summary>
        public string Highlight { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HighLightedDocument{TDoc}"/>
        /// </summary>
        public HighLightedDocument(TDoc doc, string highlight)
        {
            Doc = doc;
            Highlight = highlight;
        }
    }
}