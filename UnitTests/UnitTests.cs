using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using XamlMagic.Service;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        private string defaultConfig;
        private string legacyConfig;
        private string legacyAttributeOrderingConfig;

        [TestFixtureSetUp]
        public void Init()
        {
            this.defaultConfig = this.GetConfiguration(@"Configurations\Default.json");
            this.legacyConfig = this.GetConfiguration(@"Configurations\Legacy.json");
            this.legacyAttributeOrderingConfig = this.GetConfiguration(@"Configurations\LegacyAttributeOrdering.json");
        }

        private string GetConfiguration(string path)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }

        private void DoTest([CallerMemberName] string callerMemberName = "")
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig);
            this.DoTest(stylerOptions, callerMemberName);
        }

        private void DoTest(StylerOptions stylerOptions, [CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), null);
        }

        private void DoTestCase<T>(
            StylerOptions stylerOptions,
            T testIdentifier, 
            [CallerMemberName] string callerMemberName = "")
        {
            DoTest(stylerOptions, Path.Combine("TestFiles", callerMemberName), testIdentifier.ToString());
        }

        /// <summary>
        /// Style input document and verify output against expected  
        /// </summary>
        private static void DoTest(StylerOptions stylerOptions, string testFileBaseName, string expectedSuffix)
        {
            var stylerService = StylerService.CreateInstance(stylerOptions);
            
            var testFileResultBaseName = (expectedSuffix != null)
                ? (testFileBaseName + "_" + expectedSuffix)
                : testFileBaseName;

            // Exercise stylerService using supplied test XAML data
            string actualOutput =
                stylerService.ManipulateTreeAndFormatInput(File.ReadAllText(testFileBaseName + ".testxaml"));

            // Write output to ".actual" file for further investigation
            File.WriteAllText((testFileResultBaseName + ".actual"), actualOutput, Encoding.UTF8);

            // Check result
            Assert.That(actualOutput, Is.EqualTo(File.ReadAllText(testFileResultBaseName + ".expected")));
        }
    }
}