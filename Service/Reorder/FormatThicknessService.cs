using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XamlMagic.Service.Helpers;

namespace XamlMagic.Service.Reorder
{
    public sealed class FormatThicknessService : IProcessElementService
    {
        private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static readonly XName SetterName = XName.Get("Setter", XamlNamespace);

        public bool IsEnabled { get; }

        public ThicknessStyle ThicknessStyle { get; }

        public IList<NameSelector> ThicknessAttributeNames { get; }

        public FormatThicknessService(ThicknessStyle thicknessStyle, string thicknessAttributes)
        {
            this.IsEnabled = (thicknessStyle != ThicknessStyle.None);
            this.ThicknessStyle = thicknessStyle;
            this.ThicknessAttributeNames = thicknessAttributes.ToNameSelectorList();
        }

        public void ProcessElement(XElement element)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            if (!element.HasAttributes)
            {
                return;
            }

            // Setter? Format "Value" attribute if "Property" attribute matches ThicknessAttributeNames
            if (element.Name == SetterName)
            {
                var propertyAttribute = element.Attributes("Property").FirstOrDefault();
                if ((propertyAttribute != null)
                    && this.ThicknessAttributeNames.Any(_ => _.IsMatch(propertyAttribute.Value)))
                {
                    var valueAttribute = element.Attributes("Value").FirstOrDefault();
                    if (valueAttribute != null)
                    {
                        this.FormatAttribute(valueAttribute);
                    }
                }
            }
            // Not setter. Format value of all attributes where attribute name matches ThicknessAttributeNames
            else
            {
                foreach (var attribute in element.Attributes())
                {
                    if (this.ThicknessAttributeNames.Any(_ => _.IsMatch(attribute.Name)))
                    {
                        this.FormatAttribute(attribute);
                    }
                }
            }
        }

        private void FormatAttribute(XAttribute attribute)
        {
            char separator = (this.ThicknessStyle == ThicknessStyle.Comma) ? ',' : ' ';

            string formatted;
            if (ThicknessFormatter.TryFormat(attribute.Value, separator, out formatted))
            {
                attribute.Value = formatted;
            }
        }
    }
}