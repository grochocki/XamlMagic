using System.ComponentModel;
using XamlMagic.Service.Reorder;

namespace XamlMagic.Service.Options
{
    public class StylerOptions : IStylerOptions
    {
        public StylerOptions()
        {
            // Initialize all properties with "DefaultValueAttrbute" to their default value
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(this))
            {
                // Set default value if DefaultValueAttribute is present
                DefaultValueAttribute attribute
                    = propertyDescriptor.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

                if (attribute != null)
                {
                    propertyDescriptor.SetValue(this, attribute.Value);
                }
            }
        }

        //Indentation

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(2)]
        [Browsable(false)]
        public int IndentSize { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Browsable(false)]
        public bool IndentWithTabs { get; set; }

        // Attribute formatting

        [Category("Attribute Formatting")]
        [DisplayName("Attribute tolerance")]
        [Description("Defines the maximum number of attributes allowed on a single line. If the number of attributes exceeds this value, XamlMagic will break the attributes up across multiple lines. A value of less than 1 means always break up the attributes.\r\n\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int AttributesTolerance { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Position first attribute on same line as start tag")]
        [Description("Defines whether the first line of attribute(s) should appear on the same line as the element's start tag.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool KeepFirstAttributeOnSameLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attribute characters per line")]
        [Description("Defines the maximum character length (not including indentation characters) of attributes an element can have on each line after the start tag. A value of less than 1 means no limit.\r\n\r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int MaxAttributeCharatersPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Max attributes per line")]
        [Description("Defines the maximum number of attributes an element can have on each line after the start tag if the number of attributes exceeds the attribute tolerance. A value of less than 1 means no limit.\r\n\r\nDefault Value: 1")]
        [DefaultValue(1)]
        public int MaxAttributesPerLine { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Elements with no line breaks between attributes")]
        [Description("Defines a list of elements whose attributes should not be broken across lines.\r\n\r\nDefault Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter")]
        [DefaultValue("RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter")]
        public string NoNewLineElements { get; set; }

        [Category("Attribute Formatting")]
        [DisplayName("Put rule groups on separate lines")]
        [Description("Defines whether attributes belonging to different rule groups should be put on separate lines, while, if possible, keeping identical groups on the same line.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }
        // Attribute Reordering

        [Category("Attribute Reordering")]
        [DisplayName("Enable attribute reordering")]
        [Description("Defines whether attributes should be reordered. If false, attributes will not be reordered in any way.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool EnableAttributeReordering { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Attribute Ordering Rule Groups")]
        [Description("Defines attribute ordering rule groups. Each string element is one group. Use ',' as a delimiter between attributes. 'DOS' wildcards are allowed. XamlMagic will order attributes in groups from top to bottom, and within groups left to right.")]
        [DefaultValue(new[]
        {
            // Class definition group
            "x:Class",
            // WPF Namespaces group
            "xmlns, xmlns:x",
            // Other namespace
            "xmlns:*",
            // Element key group
            "Key, x:Key, Uid, x:Uid",
            // Element name group
            "Name, x:Name, Title",
            // Attached layout group
            "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
            // Core layout group
            "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin",
            // Alignment layout group
            "HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
            // Unmatched
            "*:*, *",
            // Miscellaneous/Other attributes group
            "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
            // Blend related group
            "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
        })]
        public string[] AttributeOrderingRuleGroups { get; set; }

        [Category("Attribute Reordering")]
        [DisplayName("Order attributes by name")]
        [Description("Defines whether attributes should be ordered by name if not determined by a rule.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool OrderAttributesByName { get; set; }

        // Element formatting

        [Category("Element Formatting")]
        [DisplayName("Put closing brackets on new line")]
        [Description("Defines whether to put closing brackets on a new line.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutEndingBracketOnNewLine { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Remove end tag of empty elements")]
        [Description("Defines whether to remove the end tag of an empty element.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool RemoveEndingTagOfEmptyElement { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Space before closing brackets in self-closing elements")]
        [Description("Defines whether there should be a space before the slash in closing brackets for self-closing elements.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool SpaceBeforeClosingSlash { get; set; }

        [Category("Element Formatting")]
        [DisplayName("Root element line breaks between attributes")]
        [Description("Defines whether attributes of the document root element are broken into multiple lines.\r\n\r\nDefault Value: Default (use same rules as other elements)")]
        [DefaultValue(LineBreakRule.Default)]
        public LineBreakRule RootElementLineBreakRule { get; set; }

        // Element reordering

        [Category("Element Reordering")]
        [DisplayName("Reorder grid panel children by row/column")]
        [Description("Defines whether to reorder the children of a Grid by row/column. When true, children will be reordered in an ascending fashion by looking first at Grid.Row, then by Grid.Column.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool ReorderGridChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder canvas panel children by left/top/right/bottom")]
        [Description("Defines whether to reorder the children of a Canvas by left/top/right/bottom. When true, children will be reordered in an ascending fashion by first at Canvas.Left, then by Canvas.Top, Canvas.Right, and finally, Canvas.Bottom.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool ReorderCanvasChildren { get; set; }

        [Category("Element Reordering")]
        [DisplayName("Reorder setters by")]
        [Description("Defines whether to reorder 'Setter' elements in style/trigger elements. When this is set, children will be reordered in an ascending fashion by looking at their Property and/or TargetName properties.\r\n\r\nDefault Value: None")]
        [DefaultValue(ReorderSettersBy.None)]
        public ReorderSettersBy ReorderSetters { get; set; }

        //Markup Extension

        [Category("Markup Extension")]
        [DisplayName("Enable markup extension formatting")]
        [Description("Defines whether to format Markup Extensions (attributes containing '{}'). When true, attributes with markup extensions will always be put on a new line, unless the element is under the attribute tolerance or one of the specified elements is in the list of elements with no line breaks between attributes.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool FormatMarkupExtension { get; set; }

        [Category("Markup Extension")]
        [DisplayName("Keep markup extensions of these types on one line")]
        [Description("Defines a comma-separated list of Markup Extensions that are always kept on a single line\r\n\r\nDefault Value: x:Bind")]
        [DefaultValue("x:Bind")]
        public string NoNewLineMarkupExtensions { get; set; }

        // Thickness formatting

        [Category("Thickness formatting")]
        [DisplayName("Thickness style")]
        [Description("Defines how thickness attributes (i.e., margin, padding, etc.) should be formatted.\r\n\r\nDefault Value: Comma")]
        [DefaultValue(ThicknessStyle.Comma)]
        public ThicknessStyle ThicknessStyle { get; set; }

        [Category("Thickness formatting")]
        [DisplayName("Thickness attributes")]
        [Description("Defines a list of attributes that get reformatted if content appears to be a thickness.\r\n\r\nDefault Value: Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        [DefaultValue("Margin, Padding, BorderThickness, ThumbnailClipMargin")]
        public string ThicknessAttributes  { get; set; }

        // Misc

        [Category("Misc")]
        [DisplayName("Format XAML on save")]
        [Description("Defines whether to automatically format the active XAML document while saving.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool BeautifyOnSave { get; set; }

        [Category("Misc")]
        [DisplayName("Number of spaces to pad comments with")]
        [Description("Determines the number of spaces a XAML comment should be padded with.\r\n\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int CommentSpaces { get; set; }
    }
}