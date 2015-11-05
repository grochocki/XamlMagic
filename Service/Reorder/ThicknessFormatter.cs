using System.Text;
using System.Text.RegularExpressions;

namespace XamlStyler.Service.Reorder
{
    public static class ThicknessFormatter
    {
        private static readonly Regex[] Capture = new Regex[]
        {
            new Regex(@"^\s*(?<c1>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*$", RegexOptions.Compiled),
            new Regex(@"^\s*(?<c1>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c2>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*$", RegexOptions.Compiled),
            new Regex(@"^\s*(?<c1>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c2>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c3>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*[ ,]\s*(?<c4>[+-]?\d*\.?\d+(?:px|in|cm|pt)?)\s*$", RegexOptions.Compiled),
        };

        public static bool TryFormat(string s, char separator, out string formatted)
        {
            foreach (var regex in Capture)
            {
                var matches = regex.Matches(s);
                if (matches.Count == 1)
                {
                    formatted = Format(matches[0], separator);
                    return true;
                }
            }

            formatted = null;
            return false;
        }

        private static string Format(Match match, char separator)
        {
            var builder = new StringBuilder();
            foreach (Group group in match.Groups)
            {
                if (group.GetType() == typeof(Group))
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(separator);
                    }
                    builder.Append(group.Value);
                }
            }

            return builder.ToString();
        }
    }
}