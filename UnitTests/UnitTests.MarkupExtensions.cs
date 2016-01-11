using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestMarkupExtensionHandling()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                FormatMarkupExtension = true,
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestMarkupWithAttributeNotOnFirstLine()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1,
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }
    }
}