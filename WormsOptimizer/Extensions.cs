using Worms;
using Worms.Utility;

namespace WormsOptimizer {
    public static class Extensions {
        public static bool IsEmpty(
            this ISimulationState self,
            Vector2Int position
        ) => false == (self.IsFood(position) || self.IsWorm(position));
    }
}