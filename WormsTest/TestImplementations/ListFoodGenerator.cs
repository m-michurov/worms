using System;
using System.Collections.Generic;
using System.Linq;
using Worms.Food;
using Worms.Utility;

namespace WormsTest.TestImplementations {
    internal sealed class ListFoodGenerator : IFoodGenerator {
        private readonly List<Vector2Int> foodPositions;
        private int stepIndex;

        public ListFoodGenerator(IEnumerable<Vector2Int> foodPositions_) => foodPositions = foodPositions_.ToList();

        public Vector2Int NextFoodPosition(Predicate<Vector2Int> isOccupied) {
            if (stepIndex >= foodPositions.Count) {
                throw new IndexOutOfRangeException("no more positions");
            }

            var position = foodPositions[stepIndex];
            stepIndex += 1;

            if (isOccupied(position)) {
                throw new ArgumentException("positions collection is invalid");
            }

            return position;
        }
    }
}