using System;
using Worms.Utility;

namespace Worms.Food {
    internal interface IFoodGenerator {
        Vector2Int NextFoodPosition(Predicate<Vector2Int> isOccupied);
    }
}