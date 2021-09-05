using System;
using Worms.Food;
using Worms.Utility;

namespace WormsTest.Food {
    internal sealed class DelegateFoodGenerator : IFoodGenerator {
        private readonly Func<Predicate<Vector2Int>, Vector2Int> f;
        public DelegateFoodGenerator(Func<Predicate<Vector2Int>, Vector2Int> f_) => f = f_;
        
        public Vector2Int NextFoodPosition(Predicate<Vector2Int> isOccupied) => f(isOccupied);
    }
}