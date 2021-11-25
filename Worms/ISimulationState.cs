using System.Collections.Generic;
using Worms.Utility;

namespace Worms {
    public interface ISimulationState {
        ICollection<Vector2Int> FoodPositions { get; }

        IDictionary<Vector2Int, int> Foods { get; }
        IEnumerable<Worm> Worms { get; }

        bool IsFood(Vector2Int position);
        bool IsWorm(Vector2Int position);
    }
}