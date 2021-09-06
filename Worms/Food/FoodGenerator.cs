using System;
using Worms.Utility;

namespace Worms.Food {
    internal sealed class FoodGenerator : IFoodGenerator {
        private const double MEAN = 0;
        internal const double STANDARD_DEVIATION = 5;

        private readonly Random random = new();

        private int NextNormal => random.NextNormal(MEAN, STANDARD_DEVIATION);

        public Vector2Int NextFoodPosition(Predicate<Vector2Int> isOccupied) {
            while (true) {
                var position = new Vector2Int(NextNormal, NextNormal);
                if (false == isOccupied(position)) {
                    return position;
                }
            }
        }
    }
}