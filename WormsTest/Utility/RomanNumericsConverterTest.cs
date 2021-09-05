using NUnit.Framework;
using Worms.Utility;

namespace WormsTest.Utility {
    public sealed class RomanNumericsConverterTest {
        [TestCase]
        public void TestConvert() {
            Assert.AreEqual("", RomanNumericsConverter.Convert(0));
            Assert.AreEqual("I", RomanNumericsConverter.Convert(1));
            Assert.AreEqual("IV", RomanNumericsConverter.Convert(4));
            Assert.AreEqual("MCMLXXXIV", RomanNumericsConverter.Convert(1984));
            Assert.AreEqual("MMMCMXCIX", RomanNumericsConverter.Convert(3999));
            Assert.AreEqual(
                RomanNumericsConverter.TOO_BIG.ToString(),
                RomanNumericsConverter.Convert(RomanNumericsConverter.TOO_BIG)
            );
            Assert.AreEqual(
                100_000L.ToString(),
                RomanNumericsConverter.Convert(100_000L)
            );
        }
    }
}