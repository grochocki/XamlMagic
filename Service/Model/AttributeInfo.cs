using System.Text.RegularExpressions;

namespace XamlMagic.Service.Model
{
    public sealed class AttributeInfo
    {
        // Fields
        private static readonly Regex MarkupExtensionPattern
            = new Regex(@"^{(?!}).*}$", (RegexOptions.Singleline | RegexOptions.Compiled));
        private static readonly Regex MarkupTypePattern
            = new Regex(@"^{(?<type>[^\s}]*)", (RegexOptions.Singleline | RegexOptions.Compiled));

        public AttributeOrderRule OrderRule { get; }
        public string Name { get; }
        public string Value { get; }
        public bool IsMarkupExtension { get; }
        public string MarkupExtension { get; }

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            this.Name = name;
            this.Value = value;
            this.IsMarkupExtension = AttributeInfo.MarkupExtensionPattern.IsMatch(value);
            this.OrderRule = orderRule;

            if (this.IsMarkupExtension)
            {
                var matchCollection = AttributeInfo.MarkupTypePattern.Matches(value);
                foreach (Match match in matchCollection)
                {
                    this.MarkupExtension = match.Groups["type"].Value;
                }
            }
        }
    }
}