using System.Collections.Generic;

namespace XamlMagic.Service.Model
{
    public sealed class MarkupExtensionInfo
    {
        /// <summary>
        /// Value could be string or MarkupExtensionInfo
        /// </summary>
        public IList<KeyValuePair<string, object>> KeyValueProperties { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Value could be string or MarkupExtensionInfo
        /// </summary>
        public IList<object> ValueOnlyProperties { get; set; }

        public MarkupExtensionInfo()
        {
            this.ValueOnlyProperties = new List<object>();
            this.KeyValueProperties = new List<KeyValuePair<string, object>>();
        }
    }
}