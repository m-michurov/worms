using System;

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
    }
}