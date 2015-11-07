using System;

namespace XamlMagic.Service.Parser
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