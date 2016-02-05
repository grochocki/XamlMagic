using NUnit.Framework;
using XamlMagic.Service.Options;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [Test]
        public void TestAttachedProperty()
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig);

            this.DoTest(stylerOptions);
        }
    }
}