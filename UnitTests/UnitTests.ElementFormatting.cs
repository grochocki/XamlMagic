using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestClosingElementHandling(int testNumber, bool spaceBeforeClosingSlash)
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            this.DoTestCase(stylerOptions, testNumber);
        }
    }
}