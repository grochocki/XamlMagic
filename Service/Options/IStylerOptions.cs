using XamlMagic.Service.Reorder;

namespace XamlMagic.Service.Options
{
    /// <summary>
    /// Options controls how Styler works
    /// </summary>
    public interface IStylerOptions
    {
        int IndentSize { get; set; }

        bool IndentWithTabs { get; set; }

        int AttributesTolerance { get; set; }

        bool KeepFirstAttributeOnSameLine { get; set; }

        int MaxAttributeCharatersPerLine { get; set; }

        int MaxAttributesPerLine { get; set; }

        string NewlineExemptionElements { get; set; }

        bool EnableAttributeReordering { get; set; }

        string[] AttributeOrderingRuleGroups { get; set; }

        bool OrderAttributesByName { get; set; }

        bool SeparateByGroups { get; set; }

        bool PutEndingBracketOnNewLine { get; set; }

        bool RemoveEndingTagOfEmptyElement { get; set; }

        bool SpaceBeforeClosingSlash { get; set; }

        LineBreakRule RootElementLineBreakRule { get; set; }

        VisualStateManagerRule ReorderVSM { get; set; }

        bool ReorderGridChildren { get; set; }

        bool ReorderCanvasChildren { get; set; }

        ReorderSettersBy ReorderSetters { get; set; }

        bool FormatMarkupExtension { get; set; }

        string NoNewLineMarkupExtensions { get; set; }

        ThicknessStyle ThicknessStyle { get; set; }

        string ThicknessAttributes { get; set; }

        bool BeautifyOnSave { get; set; }

        int CommentSpaces { get; set; }

        string ConfigPath { get; set; }
    }
}