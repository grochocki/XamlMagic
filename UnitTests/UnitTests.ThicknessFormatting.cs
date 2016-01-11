using NUnit.Framework;
using XamlMagic.Service.Options;
using XamlMagic.Service.Reorder;

namespace XamlMagic.UnitTests
{
    [TestFixture]
    public sealed partial class UnitTests
    {
        [TestCase(ThicknessStyle.None)]
        [TestCase(ThicknessStyle.Comma)]
        [TestCase(ThicknessStyle.Space)]
        public void TestThicknessHandling(ThicknessStyle thicknessStyle)
        {
            var stylerOptions = new StylerOptions(config: this.legacyConfig)
            {
                ThicknessStyle = thicknessStyle
            };

            this.DoTestCase(stylerOptions, thicknessStyle);
        }

        [TestCase(" 123", ' ', "123")]
        [TestCase(" 1.5 ", ' ', "1.5")]
        [TestCase(" 1.5 ", ',', "1.5")]
        [TestCase("1,2", ' ', "1 2")]
        [TestCase("1,2", ',', "1,2")]
        [TestCase("1 2", ',', "1,2")]
        [TestCase("1 2", ' ', "1 2")]
        [TestCase("1.5 2.5 ", ',', "1.5,2.5")]
        [TestCase("1 2 3", ' ', null)]
        [TestCase("1 2 3", ',', null)]
        [TestCase("1,2,3,4", ' ', "1 2 3 4")]
        [TestCase("1,2,3,4", ',', "1,2,3,4")]
        [TestCase("1 2 3 4", ',', "1,2,3,4")]
        [TestCase("1 2 3 4", ' ', "1 2 3 4")]
        [TestCase("1 2 3 4", ' ', "1 2 3 4")]
        [TestCase("123.456 234.567", ',', "123.456,234.567")]
        [TestCase("12 34 56 78", ',', "12,34,56,78")]
        [TestCase("1.2,3.4,5.6,7.8", ' ', "1.2 3.4 5.6 7.8")]
        [TestCase("  +12 -34  , 56 078.3", ',', "+12,-34,56,078.3")]
        [TestCase("1.2cm ", ' ', "1.2cm")]
        [TestCase("1.2mt ", ' ', null)]
        [TestCase("1.2in ,3.4cm, 5.6px,7.8pt ", ' ', "1.2in 3.4cm 5.6px 7.8pt")]
        public void TestFormatter(string testdata, char separator, string expected)
        {
            string formatted;
            var result = ThicknessFormatter.TryFormat(testdata, separator, out formatted);

            if (expected != null)
            {
                Assert.That(result, Is.True);
                Assert.That(formatted, Is.EqualTo(expected));
            }
            else
            {
                Assert.That(result, Is.False);
                Assert.That(formatted, Is.Null);
            }
        }
    }
}