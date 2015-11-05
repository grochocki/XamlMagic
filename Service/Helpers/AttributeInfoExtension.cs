using System;
using System.Reflection;
using System.Text;
using XamlStyler.Service.Model;
using XamlStyler.Service.Parser;

namespace XamlStyler.Service.Helpers
{
    internal static class AttributeInfoExtension
    {
        /// <summary>
        /// Handles markup extension value in style as:
        /// XyzAttribute="{XyzMarkup value1,
        ///                          value2,
        ///                          key1=value1,
        ///                          key2=value2}"
        /// </summary>
        /// <param name="attributeInfo"></param>
        /// <param name="baseIndentationString"></param>
        /// <returns></returns>
        public static string ToMultiLineString(this AttributeInfo attributeInfo, string baseIndentationString)
        {
            if (!attributeInfo.IsMarkupExtension)
            {
                throw new ArgumentException(
                    "AttributeInfo must have a markup extension value.",
                    MethodBase.GetCurrentMethod().GetParameters()[0].Name);
            }

            MarkupExtensionInfo info = MarkupExtensionParser.Parse(attributeInfo.Value);
            string currentIndentationString = baseIndentationString
                + String.Empty.PadLeft((attributeInfo.Name.Length + 2), ' ');
            string value = info.ToMultiLineString(currentIndentationString);

            var buffer = new StringBuilder();
            buffer.AppendFormat("{0}=\"{1}\"", attributeInfo.Name, value);

            return buffer.ToString();
        }

        /// <summary>
        /// Single line attribute line in style as:
        /// attribute_name="attribute_value"
        /// </summary>
        /// <param name="attributeInfo"></param>
        /// <returns></returns>
        public static string ToSingleLineString(this AttributeInfo attributeInfo)
        {
            string valuePart;

            if (attributeInfo.IsMarkupExtension)
            {
                var info = MarkupExtensionParser.Parse(attributeInfo.Value);
                valuePart = info.ToSingleLineString();
            }
            else
            {
                valuePart = attributeInfo.Value.ToXmlEncodedString();
            }

            return $"{attributeInfo.Name}=\"{valuePart}\"";
        }
    }
}