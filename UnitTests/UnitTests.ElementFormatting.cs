using NUnit.Framework;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestClosingElementHandling(int testNumber, bool spaceBeforeClosingSlash)
        {
            var stylerOptions = new LegacyStylerOptions
            {
                SpaceBeforeClosingSlash = spaceBeforeClosingSlash
            };

            this.DoTestCase(stylerOptions, testNumber);
        }
    }
}