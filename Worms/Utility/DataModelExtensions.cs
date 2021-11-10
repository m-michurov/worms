using System.Collections.Generic;
using System.Linq;
using Worms.Database;

namespace Worms.Utility {
    internal static class DataModelExtensions {
        private static Vector2Int ToVector2Int(this FoodPosition self) => new(self.X, self.Y);

        public static IEnumerable<Vector2Int> ToFoodPositions(this WorldBehavior self) =>
            from position in self.FoodPositions
            orderby position.Step
            select position.ToVector2Int();
    }
}