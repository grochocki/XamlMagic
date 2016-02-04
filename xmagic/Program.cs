using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using XamlMagic.Service;
using XamlMagic.Service.Options;

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

                StylerOptions stylerOptions = new StylerOptions();

                if (this.options.Configuration != null)
                {
                    var config = File.ReadAllText(this.options.Configuration);
                    stylerOptions = JsonConvert.DeserializeObject<StylerOptions>(config);

                    if (this.options.LogLevel == LogLevel.Insanity)
                    {
                        this.Log(JsonConvert.SerializeObject(stylerOptions), LogLevel.Insanity);
                    }

                    this.Log(JsonConvert.SerializeObject(stylerOptions.AttributeOrderingRuleGroups), LogLevel.Debug);
                }

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
            [OptionList('f', "files", Separator = ',', Required = true, HelpText = "XAML files to process.")]
            public IList<string> Files { get; set; }

            [Option('c', "config", HelpText = "JSON file containing XAML Magic settings configuration.")]
            public string Configuration { get; set; }

            [Option('i', "ignore", DefaultValue = false, HelpText = "Ignore XAML file type check and process all files.")]
            public bool Ignore { get; set; }

            [Option('l', "loglevel", DefaultValue = LogLevel.Default, HelpText = "Levels in order of increasing detail: None, Minimal, Default, Verbose, Debug")]
            public LogLevel LogLevel { get; set; }

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
