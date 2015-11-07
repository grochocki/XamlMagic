using System.Text.RegularExpressions;

namespace XamlMagic.Service
{
    public class XmlEscapingService
    {
        private readonly Regex HtmlReservedCharRegex = new Regex(@"&([\d\D][^;]{3,7});");
        private readonly Regex HtmlReservedCharRestoreRegex = new Regex(@"__amp__([^;]{2,7})__scln__");

        protected string EscapeDocument(string source)
        {
            return HtmlReservedCharRegex.Replace(source, @"__amp__$1__scln__");
        }

        protected string UnescapeDocument(string source)
        {
            return HtmlReservedCharRestoreRegex.Replace(source, @"&$1;");
        }
    }
}