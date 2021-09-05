using System;
using System.Text;

namespace Worms.Utility {
    internal static class RomanNumericsConverter {
        internal const long TOO_BIG = 4000;

        private static readonly string[] romanNumerals = {
            "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"
        };

        private static readonly long[] numerals = {
            1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1
        };

        static RomanNumericsConverter() {
            if (romanNumerals.Length != numerals.Length) {
                throw new ApplicationException("numeral array's lengths do not match");
            }
        }

        /// <summary>
        ///     Convert a number to a roman numerals representation
        /// </summary>
        /// <param name="number">The number to convert</param>
        /// <returns>
        ///     Roman numerals representation or base 10 representation if the number is too big (>= <see cref="TOO_BIG" />)
        /// </returns>
        internal static string Convert(long number) {
            if (number >= TOO_BIG) {
                return number.ToString();
            }

            var result = new StringBuilder("");

            for (var numeralIndex = 0; numeralIndex < numerals.Length; numeralIndex += 1) {
                var numeral = numerals[numeralIndex];
                var repeatTimes = number / numeral;

                if (0 == repeatTimes) {
                    continue;
                }

                var romanNumeral = romanNumerals[numeralIndex];

                for (var repeat = 0; repeat < repeatTimes; repeat += 1) {
                    result.Append(romanNumeral);
                }

                number -= numeral * repeatTimes;
            }

            return result.ToString();
        }
    }
}