using NUnit.Framework;
using System.IO;
using System.Text;
using XamlMagic.Service;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
        {
        private void DoTest([System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            this.DoTest(new LegacyStylerOptions(), callerMemberName);
        }

        private void DoTest(StylerOptions stylerOptions, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), null);
        }

        private void DoTestCase<T>(StylerOptions stylerOptions, T testIdentifier, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), testIdentifier.ToString());
        }

        /// <summary>
        /// Style input document and verify output against expected  
        /// </summary>
        private static void DoTest(StylerOptions stylerOptions, string testFileBaseName, string expectedSuffix)
        {
            var stylerService = StylerService.CreateInstance(stylerOptions);
            
            var testFileResultBaseName = expectedSuffix != null ? testFileBaseName + "_" + expectedSuffix : testFileBaseName;

            // Exercise stylerService using supplied test XAML data
            string actualOutput = stylerService.ManipulateTreeAndFormatInput(File.ReadAllText(testFileBaseName + ".testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText(testFileResultBaseName + ".actual", actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText(testFileResultBaseName + ".expected")));
        }

        // TODO - Update tests to work regardless of default settings
        private class LegacyStylerOptions : StylerOptions
        {
            public LegacyStylerOptions() : base()
            {
                this.AttributeOrderingRuleGroups = LegacyStylerOptions.LegacyAttributeOrderingRuleGroups;
                this.KeepFirstAttributeOnSameLine = true;
                this.ReorderGridChildren = true;
                this.ReorderCanvasChildren = true;
            }

            public static string[] LegacyAttributeOrderingRuleGroups = new[]
            {
                "x:Class",
                "xmlns, xmlns:x",
                "xmlns:*",
                "Key, x:Key, Uid, x:Uid",
                "Name, x:Name, Title",
                "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
                "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin",
                "HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
                "*:*, *",
                "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
                "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
            };
        }
    }
}