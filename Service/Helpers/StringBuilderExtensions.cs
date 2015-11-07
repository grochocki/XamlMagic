using System.Linq;
using System.Text;

namespace XamlMagic.Service.Helpers
{
    public static class StringBuilderExtensions
    {
        public static bool IsNewLine(this StringBuilder builder)
        {
            return (builder.Length > 0) && (builder[builder.Length - 1] == '\n');
        }

        /// <summary>
        /// Get index of last occurence of char
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LastIndexOf(this StringBuilder builder, char value)
        {
            for (int i = builder.Length-1; i >= 0; i--)
            {
                if (builder[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string Substring(this StringBuilder builder, int startIndex, int length)
        {
            return builder.ToString(startIndex, length);
        }

        public static StringBuilder TrimEnd(this StringBuilder builder, params char[] trimChars)
        {
            int index = builder.Length;
            while ((index > 0) && trimChars.Contains(builder[index-1]))
            {
                index--;
            }
            builder.Length = index;
            return builder;
        }

        /// <summary>
        /// Trim all trimchars from end of stringbuilder except if escaped
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="trimChars"></param>
        public static StringBuilder TrimUnescaped(this StringBuilder builder, params char[] trimChars)
        {
            int index = builder.Length;
            while ((index > 0) && trimChars.Contains(builder[index-1]))
            {
                if ((index > 1) && (builder[index - 2] == '\\'))
                {
                    break;
                }
                index--;
            }
            builder.Length = index;
            return builder;
        }
    }
}
