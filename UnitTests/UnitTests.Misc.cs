using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestDefaultHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestNoContentElementHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestTextOnlyContentElementHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestNestedPropertiesAndChildrenHandling()
        {
            var stylerOptions = new LegacyStylerOptions
            {
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTest(stylerOptions);
        }

        [TestCase(1, LineBreakRule.Default)]
        [TestCase(2, LineBreakRule.Always)]
        [TestCase(3, LineBreakRule.Never)]
        public void TestRootHandling(int testNumber, LineBreakRule lineBreakRule)
        {
            var stylerOptions = new LegacyStylerOptions
            {
                AttributesTolerance = 3,
                MaxAttributesPerLine = 4,
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                RootElementLineBreakRule = lineBreakRule,
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestRunHandling()
        {
            this.DoTest();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestCommentHandling(byte testNumber)
        {
            var stylerOptions = new LegacyStylerOptions
            {
                CommentSpaces = testNumber,
            };

            this.DoTestCase(stylerOptions, testNumber);
        }

        [Test]
        public void TestCDATAHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestXmlSpaceHandling()
        {
            this.DoTest();
        }
    }
}