using System;
using System.Collections.Generic;
using Worms.Utility;

namespace Worms.Names {
    internal sealed class NameGenerator : INameGenerator {
        private static readonly string[] baseNames = {
            "Alfred",
            "Edward",
            "Edmund",
            "Eadwig",
            "Harold",
            "William",
            "Henry",
            "Stephen",
            "Richard",
            "John",
            "Louis",
            "Jane",
            "Philip"
        };

        private readonly Dictionary<string, long> numbers = new();

        private readonly Random random = new();

        internal NameGenerator() {
            foreach (var name in baseNames) {
                numbers[name] = 1;
            }
        }

        public string NextName => GenerateName();

        private string GenerateName() {
            var baseName = baseNames[random.Next(0, baseNames.Length)];
            var number = numbers[baseName];
            numbers[baseName] += 1;
            var romanNumber = RomanNumericsConverter.Convert(number);

            return $"{baseName} {romanNumber}";
        }
    }
}