using System.Collections.Generic;
using Worms.Utility;

namespace Worms {
    public interface ISimulationState {
        ICollection<Vector2Int> FoodPositions { get; }

        IEnumerable<string> Foods { get; }
        IEnumerable<string> Worms { get; }

        bool IsFood(Vector2Int position);
        bool IsWorm(Vector2Int position);
    }
}