using System;
using FluentAssertions;
using Worms.Utility;
using Xunit;

namespace WormsTest.Utility {
    public sealed class RomanNumericsConverterTests {
        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "I")]
        [InlineData(4, "IV")]
        [InlineData(1984, "MCMLXXXIV")]
        [InlineData(3999, "MMMCMXCIX")]
        public void Small_numbers_are_converted_to_roman_numerals(
            long number,
            string expected
        ) {
            var converted = RomanNumericsConverter.Convert(number);
            converted.Should().Be(expected);
        }

        [Theory]
        [InlineData(RomanNumericsConverter.TOO_BIG)]
        [InlineData(1_000_000)]
        public void Large_numbers_are_converted_to_base_10_strings(long number) {
            var converted = RomanNumericsConverter.Convert(number);
            converted.Should().Be(number.ToString());
        }

        [Fact]
        public void Negative_numbers_are_not_converted() {
            var negativeNumberConversion = (Action) (() => RomanNumericsConverter.Convert(-1));
            negativeNumberConversion.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}