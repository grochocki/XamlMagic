using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestAttributeThresholdHandling()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                AttributesTolerance = 0,
                MaxAttributeCharatersPerLine = 80,
                MaxAttributesPerLine = 3,
                PutEndingBracketOnNewLine = true
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeOrderRuleGroupsOnSeparateLinesHandling()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                PutAttributeOrderRuleGroupsOnSeparateLines = true,
                MaxAttributesPerLine = 3,
            };

            this.DoTest(stylerOptions);
        }

        [Test]
        public void TestAttributeToleranceHandling()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                AttributesTolerance = 3,
                RootElementLineBreakRule = LineBreakRule.Always,
            };

            this.DoTest(stylerOptions);
        }
    }
}