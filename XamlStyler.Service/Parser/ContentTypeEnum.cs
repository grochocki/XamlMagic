using System;

namespace XamlStyler.Service.Parser
{
    [Flags]
    public enum ContentTypeEnum
    {
        None = 0,
        SingleLineTextOnly = 1,
        MultiLineTextOnly = 2,
        Mixed = 4,
    }
}