using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace XamlStyler.Service.Reorder
{
    public sealed class SortBy: NameSelector
    {
        private Func<XElement,string> defaultValue;

        private bool isNumeric;
        [DisplayName("Namespace")]
        [Description("Match name by namespace. null/empty = all. 'DOS' Wildcards permitted.")]
        public bool IsNumeric { 
            get { return isNumeric; }
            set
            {
                isNumeric = value;
                defaultValue = this.IsNumeric
                    ? (Func<XElement, string>) (_ => _.Name.LocalName.Contains(".") ? "-32768" : "-32767")
                    : (_ => "");
            } 
        }

        public SortBy(string name, string @namespace, bool isNumeric)
            : base(name, @namespace)
        {
            this.IsNumeric = isNumeric;
        }

        public SortBy(string name, bool isNumeric)
            : base(name)
        {
            this.IsNumeric = isNumeric;
        }


        public ISortableAttribute GetValue(XElement element)
        {
            var attribute = element.Attributes().FirstOrDefault(_ => IsMatch(_.Name));
            string value = null;
            if (attribute != null)
            {
                value = attribute.Value;
            }

            return this.IsNumeric 
                ? (ISortableAttribute) new SortableNumericAttribute(value, Double.Parse(defaultValue(element)))
                : (ISortableAttribute) new SortableStringAttribute(value ?? defaultValue(element));
        }
    }
}