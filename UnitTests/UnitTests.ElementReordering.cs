using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestGridChildrenHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestNestedGridChildrenHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestCanvasChildrenHandling()
        {
            this.DoTest();
        }

        [Test]
        public void TestNestedCanvasChildrenHandling()
        {
            this.DoTest();
        }

        [TestCase(ReorderSettersBy.Property)]
        [TestCase(ReorderSettersBy.TargetName)]
        [TestCase(ReorderSettersBy.TargetNameThenProperty)]
        public void TestReorderSetterHandling(ReorderSettersBy reorderSettersBy)
        {
            var stylerOptions = new LegacyStylerOptions
            {
                ReorderSetters = reorderSettersBy,
                NoNewLineMarkupExtensions = "x:Bind"
            };

            this.DoTestCase(stylerOptions, reorderSettersBy);
        }

        [Test]
        public void TestVisualStateManagerDefault()
        {
            var stylerOptions = new StylerOptions
            {
                AttributeOrderingRuleGroups = LegacyStylerOptions.LegacyAttributeOrderingRuleGroups
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestVisualStateManagerFirst()
        {
            var stylerOptions = new StylerOptions
            {
                AttributeOrderingRuleGroups = LegacyStylerOptions.LegacyAttributeOrderingRuleGroups,
                ReorderVSM = VisualStateManagerRule.First
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestVisualStateManagerLast()
        {
            var stylerOptions = new StylerOptions
            {
                AttributeOrderingRuleGroups = LegacyStylerOptions.LegacyAttributeOrderingRuleGroups,
                ReorderVSM = VisualStateManagerRule.Last
            };

            this.DoTest(stylerOptions);
        }
    }
}