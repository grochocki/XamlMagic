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
            var stylerOptions = new StylerOptions(config: this.defaultConfig)
			{
				IndentWithTabs = true
			};

            this.DoTest(stylerOptions);
        }
    }
}