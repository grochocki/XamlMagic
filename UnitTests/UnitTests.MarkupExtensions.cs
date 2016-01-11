using NUnit.Framework;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestxBindSplitting()
        {
            var stylerOptions = new LegacyStylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestBindingSplitting()
        {
            var stylerOptions = new LegacyStylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind, Binding"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestMarkupExtensionHandling()
        {
            var stylerOptions = new LegacyStylerOptions
            {
                FormatMarkupExtension = true,
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestMarkupWithAttributeNotOnFirstLine()
        {
            var stylerOptions = new LegacyStylerOptions
            {
                KeepFirstAttributeOnSameLine = false,
                AttributesTolerance = 1,
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }
    }
}