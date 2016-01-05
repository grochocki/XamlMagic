using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XamlMagic.Service;
using XamlMagic.Service.Options;
using XamlMagic.Service.Reorder;

namespace Xmagic
{
    public sealed class Program
    {
        public sealed class XamlMagicConsole
        {
            private readonly Options options;
            private readonly StylerService stylerService;

            public XamlMagicConsole(Options options)
            {
                this.options = options;

                var stylerOptions = new StylerOptions
                {
                    AttributeOrderingRuleGroups = options.AttributeOrderingRuleGroups?.ToArray() ?? Options.DefaultAttributeOrderingRuleGroups.ToArray(),
                    AttributesTolerance = options.AttributesTolerance,
                    CommentSpaces = options.CommentSpaces,
                    EnableAttributeReordering = options.EnableAttributeReordering,
                    FormatMarkupExtension = options.FormatMarkupExtension,
                    KeepFirstAttributeOnSameLine = options.KeepFirstAttributeOnSameLine,
                    MaxAttributeCharatersPerLine = options.MaxAttributeCharatersPerLine,
                    MaxAttributesPerLine = options.MaxAttributesPerLine,
                    NoNewLineElements = String.Join(",", options.NoNewLineElements ?? Options.DefaultNoNewLineElements),
                    NoNewLineMarkupExtensions = String.Join(",", options.NoNewLineMarkupExtensions ?? Options.DefaultNoNewLineMarkupExtensions),
                    OrderAttributesByName = options.OrderAttributesByName,
                    PutAttributeOrderRuleGroupsOnSeparateLines = options.PutAttributeOrderRuleGroupsOnSeparateLines,
                    PutEndingBracketOnNewLine = options.PutEndingBracketOnNewLine,
                    RemoveEndingTagOfEmptyElement = options.RemoveEndingTagOfEmptyElement,
                    ReorderCanvasChildren = options.ReorderCanvasChildren,
                    ReorderGridChildren = options.ReorderGridChildren,
                    ReorderSetters = options.ReorderSetters,
                    ReorderVSM = options.ReorderVSM,
                    RootElementLineBreakRule = options.RootElementLineBreakRule,
                    SpaceBeforeClosingSlash = options.SpaceBeforeClosingSlash,
                    ThicknessAttributes = String.Join(",", options.ThicknessAttributes ?? Options.DefaultThicknessAttributes),
                    ThicknessStyle = options.ThicknessStyle
                };

                this.stylerService = StylerService.CreateInstance(stylerOptions);
            }

            public void Process()
            {
                int successCount = 0;

                foreach (string file in this.options.Files)
                {
                    this.Log($"Processing: {file}");

                    if (!options.Ignore)
                    {
                        var extension = Path.GetExtension(file);
                        this.Log($"Extension: {extension}", LogLevel.Debug);

                        if (!extension.Equals(".xaml", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Log("Skipping... Can only process XAML files. Use the --ignore parameter to override.");
                            continue;
                        }
                    }

                    var path = Path.GetFullPath(file);
                    this.Log($"Full Path: {file}", LogLevel.Debug);

                    var originalContent = File.ReadAllText(file);
                    this.Log($"\nOriginal Content:\n\n{originalContent}\n", LogLevel.Insanity);

                    var formattedOutput = stylerService.ManipulateTreeAndFormatInput(originalContent);
                    this.Log($"\nFormatted Output:\n\n{formattedOutput}\n", LogLevel.Insanity);

                    try
                    {
                        File.WriteAllText(file, formattedOutput);
                        this.Log($"Finished Processing: {file}", LogLevel.Verbose);
                        successCount++;
                    }
                    catch(Exception e)
                    {
                        this.Log("Skipping... Error formatting XAML. Increase log level for more details.");
                        this.Log($"Exception: {e.Message}", LogLevel.Verbose);
                        this.Log($"StackTrace: {e.StackTrace}", LogLevel.Debug);
                    }
                }

                this.Log($"Processed {successCount} of {this.options.Files.Count} files.", LogLevel.Minimal);
            }

            private void Log(string value, LogLevel logLevel = LogLevel.Default)
            {
                if (logLevel <= this.options.LogLevel)
                {
                    Console.WriteLine(value);
                }
            }
        }

        public static void Main(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArgumentsStrict(args, options);
            var xamlMagicConsole = new XamlMagicConsole(options);
            xamlMagicConsole.Process();
        }

        public sealed class Options
        {
            public static List<string> DefaultNoNewLineElements = new List<string>()
            {
                "RadialGradientBrush",
                "GradientStop",
                "LinearGradientBrush",
                "ScaleTransfom",
                "SkewTransform",
                "RotateTransform",
                "TranslateTransform",
                "Trigger",
                "Condition",
                "Setter"
            };

            public static List<string> DefaultNoNewLineMarkupExtensions = new List<string>() { "x:Bind", "Binding" };

            public static List<string> DefaultThicknessAttributes = new List<string>()
            {
                "Margin",
                "Padding",
                "BorderThickness",
                "ThumbnailClipMargin"
            };

            public static List<string> DefaultAttributeOrderingRuleGroups = new List<string>()
            {
                "x:Class",
                "xmlns, xmlns:x",
                "xmlns:*",
                "Key, x:Key, Uid, x:Uid",
                "Name, x:Name, Title",
                "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
                "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin",
                "Margin, Padding, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
                "*:*, *",
                "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
                "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText"
            };

            [OptionList('f', "files", Separator = ',', Required = true, HelpText = "XAML files to process.")]
            public IList<string> Files { get; set; }

            [Option('i', "ignore", DefaultValue = false, HelpText = "Ignore XAML file type check and process all files.")]
            public bool Ignore { get; set; }

            [Option('l', "loglevel", DefaultValue = LogLevel.Default, HelpText = "Levels in order of increasing detail: None, Minimal, Default, Verbose, Debug")]
            public LogLevel LogLevel { get; set; }

            [Option("attributesTolerance", DefaultValue = 2)]
            public int AttributesTolerance { get; set; }

            [Option("commentSpaces", DefaultValue = 2)]
            public int CommentSpaces { get; set; }

            [Option("enableAttributeReordering", DefaultValue = true)]
            public bool EnableAttributeReordering { get; set; }

            [OptionList("noNewLineMarkupExtensions", Separator = ',', DefaultValue = null, HelpText = "Default is same as XAML Magic extension")]
            public IList<string> NoNewLineMarkupExtensions { get; set; }

            [Option("formatMarkupExtension", DefaultValue = true)]
            public bool FormatMarkupExtension { get; set; }

            [Option("keepFirstAttributeOnSameLine", DefaultValue = false)]
            public bool KeepFirstAttributeOnSameLine { get; set; }

            [Option("maxAttributeCharatersPerLine", DefaultValue = 0)]
            public int MaxAttributeCharatersPerLine { get; set; }

            [Option("maxAttributesPerLine", DefaultValue = 1)]
            public int MaxAttributesPerLine { get; set; }

            [OptionList("noNewLineElements", Separator = ',', DefaultValue = null, HelpText = "Default is same as XAML Magic extension")]
            public IList<string> NoNewLineElements { get; set; }

            [OptionList("attributeOrderingRuleGroups", Separator = '?', DefaultValue = null, HelpText = "Default is same as XAML Magic extension. Deliminate between groups with '?' and within groups with ','. Do not insert spaces.")]
            public IList<string> AttributeOrderingRuleGroups { get; set; }

            [Option("orderAttributesByName", DefaultValue = true)]
            public bool OrderAttributesByName { get; set; }

            [Option("putAttributeOrderRuleGroupsOnSeparateLines", DefaultValue = false)]
            public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; }

            [Option("putEndingBracketOnNewLine", DefaultValue = false)]
            public bool PutEndingBracketOnNewLine { get; set; }

            [Option("removeEndingTagOfEmptyElement", DefaultValue = true)]
            public bool RemoveEndingTagOfEmptyElement { get; set; }

            [Option("reorderCanvasChildren", DefaultValue = false)]
            public bool ReorderCanvasChildren { get; set; }

            [Option("reorderGridChildren", DefaultValue = false)]
            public bool ReorderGridChildren { get; set; }

            [Option("reorderSetters", DefaultValue = ReorderSettersBy.None)]
            public ReorderSettersBy ReorderSetters { get; set; }

            [Option("reorderVSM", DefaultValue = VisualStateManagerRule.None)]
            public VisualStateManagerRule ReorderVSM { get; set; }

            [Option("rootElementLineBreakRule", DefaultValue = LineBreakRule.Default)]
            public LineBreakRule RootElementLineBreakRule { get; set; }

            [Option("spaceBeforeClosingSlash", DefaultValue = true)]
            public bool SpaceBeforeClosingSlash { get; set; }

            [OptionList("thicknessAttributes", Separator = ',', DefaultValue = null, HelpText = "Default: Margin, Padding, BorderThickness, ThumbnailClipMargin")]
            public IList<string> ThicknessAttributes { get; set; }

            [Option("thicknessStyle", DefaultValue = ThicknessStyle.Comma)]
            public ThicknessStyle ThicknessStyle { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this);
            }
        }

        public enum LogLevel
        {
            None = 0,
            Minimal = 1,
            Default = 2,
            Verbose = 3,
            Debug = 4,
            Insanity = 5,
        }
    }
}
