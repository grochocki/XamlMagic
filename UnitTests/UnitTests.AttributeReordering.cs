using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestAttributeSortingOptionHandling()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                AttributeOrderingRuleGroups = new[]
                {
                    "x:Class*",
                    "xmlns, xmlns:x",
                    "xmlns:*",
                    "Key, x:Key, Uid, x:Uid",
                    "Name, x:Name, Title",
                    "Grid.Column, Grid.ColumnSpan, Grid.Row, Grid.RowSpan, Canvas.Right, Canvas.Bottom, Canvas.Left, Canvas.Top",
                    "MinWidth, MinHeight, Width, Height, MaxWidth, MaxHeight, Margin",
                    "Panel.ZIndex, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment",
                    "*:*, *",
                    "Offset, Color, TargetName, Property, Value, StartPoint, EndPoint, PageSource, PageIndex",
                    "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
                }
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestWildCard()
        {
            var stylerOptions = new StylerOptions(config: this.defaultConfig)
            {
                AttributeOrderingRuleGroups = new[]
                {
                    "x:Class*",
                    "xmlns, xmlns:x",
                    "xmlns:*",
                    "x:Key, Key, x:Name, Name, x:Uid, Uid, Title",
                    "Grid.*, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
                    "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight",
                    "Margin, Padding, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
                    "Style, Background, Foreground, Fill, BorderBrush, BorderThickness, Stroke, StrokeThickness, Opacity",
                    "FontFamily, FontSize, LineHeight, FontWeight, FontStyle, FontStretch",
                    "*:*, *",
                    "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
                    "ToolTipService.*, AutomationProperties.*",
                    "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText"
                }
            };
            this.DoTest(stylerOptions);
        }
    }
}