using System;
using System.Collections.Generic;

namespace Worms.Utility {
    internal static class RandomExtensions {
        internal static bool NextBool(
            this Random random,
            double probability
        ) {
            if (probability is < 0 or > 1) {
                throw new ArgumentOutOfRangeException(nameof(probability));
            }

            return random.NextDouble() < probability;
        }

        internal static int NextNormal(
            this Random random,
            double mean = 0,
            double standardDeviation = 1
        ) {
            var u1 = random.NextDouble();
            var u2 = random.NextDouble();

            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            var randNormal = mean + standardDeviation * randStdNormal;

            return (int) Math.Round(randNormal);
        }

        private static T Choose<T>(
            this Random random,
            IList<T> elements
        ) =>
            elements[random.Next(0, elements.Count)];

        internal static Direction NextDirection(this Random random) => random.Choose(Direction.AllDirections);
    }
}