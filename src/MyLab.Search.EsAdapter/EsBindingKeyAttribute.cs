using System;

namespace MyLab.Search.EsAdapter
{
    /// <summary>
    /// Specifies string key for binding
    /// </summary>
    public class EsBindingKeyAttribute : Attribute
    {
        /// <summary>
        /// Key for binding
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EsBindingKeyAttribute"/>
        /// </summary>
        public EsBindingKeyAttribute(string key)
        {
            Key = key;
        }
    }
}
