namespace XamlStyler.Service.Parser
{
    public enum MarkupExtensionParsingModeEnum
    {
        Start,
        MarkupName,
        NameValuePair,
        MarkupExtensionValue,
        QuotedLiteralValue,
        LiteralValue,
        End,
        Unexpected,
    }
}