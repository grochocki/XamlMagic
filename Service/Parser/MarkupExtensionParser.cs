using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XamlMagic.Service.Helpers;
using XamlMagic.Service.Model;

namespace XamlMagic.Service.Parser
{
    internal static class MarkupExtensionParser
    {
        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");

        public static MarkupExtensionInfo Parse(string input)
        {
            if (!MarkupExtensionPattern.IsMatch(input))
            {
                throw new InvalidOperationException($"{input} is not a MarkupExtension.");
            }

            var resultInfo = new MarkupExtensionInfo();

            using (var reader = new StringReader(input))
            {
                var parsingMode = MarkupExtensionParsingModeEnum.Start;

                try
                {
                    //Debug.Print("Parsing '{0}'", input);
                    //Debug.Indent();

                    while ((MarkupExtensionParsingModeEnum.End != parsingMode)
                        && (MarkupExtensionParsingModeEnum.Unexpected != parsingMode))
                    {
                        //Debug.Print(context.ToString());
                        //Debug.Indent();

                        switch (parsingMode)
                        {
                            case MarkupExtensionParsingModeEnum.Start:
                                parsingMode = reader.ReadMarkupExtensionStart();
                                break;
                            case MarkupExtensionParsingModeEnum.MarkupName:
                                parsingMode = reader.ReadMarkupName(resultInfo);
                                break;
                            case MarkupExtensionParsingModeEnum.NameValuePair:
                                parsingMode = reader.ReadNameValuePair(resultInfo);
                                break;
                        }

                        //Debug.Unindent();
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidDataException($"Cannot parse markup extension string:\r\n \"{input}\"", e);
                }
            }

            return resultInfo;
        }

        // TODO: move following extension methods to helper namespace
        private static bool IsEnd(this StringReader reader)
        {
            return (reader.Peek() < 0);
        }

        private static char PeekChar(this StringReader reader)
        {
            var result = (char) reader.Peek();

            //Debug.Print("?Peek '{0}'", result);

            return result;
        }

        private static char ReadChar(this StringReader reader)
        {
            var result = (char) reader.Read();

            //Debug.Print("!Read '{0}'", result);

            return result;
        }

        private static MarkupExtensionParsingModeEnum ReadMarkupExtensionStart(this StringReader reader)
        {
            reader.SeekTill(_ => ('{' != _) && !Char.IsWhiteSpace(_));

            return MarkupExtensionParsingModeEnum.MarkupName;
        }

        private static MarkupExtensionParsingModeEnum ReadMarkupName(this StringReader reader, MarkupExtensionInfo info)
        {
            char[] stopChars = {' ', '}'};
            var resultParsingMode = MarkupExtensionParsingModeEnum.Unexpected;
            var buffer = new StringBuilder();

            while (!reader.IsEnd())
            {
                char c = reader.ReadChar();

                if (stopChars.Contains(c))
                {
                    switch (c)
                    {
                        case ' ':
                            resultParsingMode = MarkupExtensionParsingModeEnum.NameValuePair;
                            break;
                        case '}':
                            resultParsingMode = MarkupExtensionParsingModeEnum.End;
                            break;
                        default:
                            throw new InvalidDataException($"[{nameof(ReadMarkupName)}] Should not encounter '{c}'.");
                    }

                    info.Name = buffer.ToString().Trim();
                    buffer.Clear();

                    break;
                }
                
                buffer.Append(c);
            }

            if (MarkupExtensionParsingModeEnum.Unexpected == resultParsingMode)
            {
                throw new InvalidDataException($"[{nameof(ReadMarkupName)}] Invalid result context: {resultParsingMode}");
            }

            return resultParsingMode;
        }

        private static MarkupExtensionParsingModeEnum ReadNameValuePair(
            this StringReader reader,
            MarkupExtensionInfo info)
        {
            char[] stopChars = {',', '=', '}'};

            MarkupExtensionParsingModeEnum resultParsingMode;
            string key = null;
            object value = null;

            reader.SeekTill(_ => !Char.IsWhiteSpace(_));

            // When '{' is the starting char, the following must be a value instead of a key.
            //
            // E.g.,
            //    <Setter x:Uid="Setter_75"
            //            Property="Foreground"
            //            Value="{DynamicResource {x:Static SystemColors.ControlTextBrushKey}}" />
            //
            // In other words, "key" shall not start with '{', as it won't be a valid property name.
            if (reader.PeekChar() != '{')
            {
                string temp = reader.ReadTill(stopChars.Contains).Trim();
                char keyValueIndicatorChar = reader.PeekChar();

                switch (keyValueIndicatorChar)
                {
                    case ',':
                    case '}':
                        value = temp;
                        break;
                    case '=':
                        key = temp;
                        // Consume the '='
                        reader.Read();
                        break;
                    default:
                        throw new InvalidDataException($"[{nameof(ReadNameValuePair)}] Should not encounter '{keyValueIndicatorChar}'.");
                }
            }

            if (value == null)
            {
                reader.SeekTill(_ => !(Char.IsWhiteSpace(_)));

                string input = reader.ReadValueString();

                if (MarkupExtensionPattern.IsMatch(input))
                {
                    value = Parse(input);
                }
                else
                {
                    value = input;
                }
            }

            if (String.IsNullOrEmpty(key))
            {
                info.ValueOnlyProperties.Add(value);
            }
            else
            {
                info.KeyValueProperties.Add(new KeyValuePair<string, object>(key, value));
            }

            reader.SeekTill(_ => !Char.IsWhiteSpace(_));

            char stopChar = reader.ReadChar();

            switch (stopChar)
            {
                case ',':
                    resultParsingMode = MarkupExtensionParsingModeEnum.NameValuePair;
                    break;
                case '}':
                    resultParsingMode = MarkupExtensionParsingModeEnum.End;
                    break;
                default:
                    throw new InvalidDataException($"[{nameof(ReadNameValuePair)}] Should not encounter '{stopChar}'.");
            }

            if (MarkupExtensionParsingModeEnum.Unexpected == resultParsingMode)
            {
                throw new InvalidDataException($"[{nameof(ReadNameValuePair)}] Invalid result context: {resultParsingMode}");
            }

            return resultParsingMode;
        }

        private static string ReadTill(this StringReader reader, Func<char, bool> stopAt)
        {
            var buffer = new StringBuilder();

            while (!reader.IsEnd())
            {
                if (stopAt((char) reader.Peek()))
                {
                    break;
                }
                buffer.Append(reader.ReadChar());
            }

            if (reader.IsEnd())
            {
                throw new InvalidDataException($"[{nameof(ReadTill)}] Cannot meet the stop condition.");
            }

            return buffer.ToString();
        }

        private static string ReadValueString(this StringReader reader)
        {
            var buffer = new StringBuilder();
            int curlyBracePairCounter = 0;
            MarkupExtensionParsingModeEnum parsingMode;

            // ignore leading spaces
            reader.SeekTill(_ => !Char.IsWhiteSpace(_));

            // Determine parsing mode
            char c = reader.ReadChar();
            buffer.Append(c);

            if (c == '{')
            {
                char peek = reader.PeekChar();

                parsingMode = ('}' != peek)
                    ? MarkupExtensionParsingModeEnum.MarkupExtensionValue
                    : MarkupExtensionParsingModeEnum.LiteralValue;

                curlyBracePairCounter++;
            }
            else if (c == '\'')
            {
                parsingMode = MarkupExtensionParsingModeEnum.QuotedLiteralValue;
            }
            else
            {
                parsingMode = MarkupExtensionParsingModeEnum.LiteralValue;
            }

            switch (parsingMode)
            {
                case MarkupExtensionParsingModeEnum.MarkupExtensionValue:
                    while ((curlyBracePairCounter > 0) && !reader.IsEnd())
                    {
                        c = reader.ReadChar();
                        buffer.Append(c);

                        switch (c)
                        {
                            case '{':
                                curlyBracePairCounter++;
                                break;
                            case '}':
                                curlyBracePairCounter--;
                                break;
                        }
                    }
                    break;
                case MarkupExtensionParsingModeEnum.QuotedLiteralValue:
                    // Following case is handled:
                    //      StringFormat='{}{0}\'s email'
                    do
                    {
                        buffer.Append(reader.ReadTill(_ => (_ == '\'')));
                        buffer.Append(reader.ReadChar());
                    } while ((buffer.Length > 2)
                        && (buffer[buffer.Length - 1] == '\'')
                        && (buffer[buffer.Length - 2] == '\\'));

                    break;
                case MarkupExtensionParsingModeEnum.LiteralValue:
                    bool shouldStop = false;

                    while (!reader.IsEnd())
                    {
                        switch (reader.PeekChar())
                        {
                            case '{':
                                curlyBracePairCounter++;
                                break;
                            case '}':
                                if (curlyBracePairCounter > 0)
                                {
                                    curlyBracePairCounter--;
                                }
                                else
                                {
                                    shouldStop = true;
                                }
                                break;
                            // Escape character
                            case '\\':
                                buffer.Append(reader.ReadChar());
                                break;
                            case ',':
                                shouldStop = (curlyBracePairCounter == 0);
                                break;
                        }

                        if (!shouldStop)
                        {
                            buffer.Append(reader.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                default:
                    throw new InvalidDataException($"[{nameof(ReadValueString)}] Should not encouter parsingMode {parsingMode}");
            }

            return buffer.TrimUnescaped(' ').ToString();
        }

        private static void SeekTill(this StringReader reader, Func<char, bool> stopAt)
        {
            while (!reader.IsEnd())
            {
                if (stopAt((char) reader.Peek()))
                {
                    break;
                }
                reader.ReadChar();
            }

            if (reader.IsEnd())
            {
                throw new InvalidDataException($"[{nameof(SeekTill)}] Cannot meet the stop condition.");
            }
        }
    }
}