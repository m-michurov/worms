using System;
using System.Collections.Generic;
using System.Linq;
using Worms;
using Worms.DTO;
using Worms.Utility;
using Worm = Worms.Worm;

namespace WormsServer.DTO {
    internal static class Extensions {
        private static Vector2Int ToVector2Int(this Position self) => new(self.X, self.Y);

        private static Worm ToWorm(this Worms.DTO.Worm self) =>
            new(self.Name, self.Position.ToVector2Int(), self.Energy);

        private static IEnumerable<Worm> ToWormsEnumerable(this IEnumerable<Worms.DTO.Worm> self) =>
            self.Select(it => it.ToWorm());

        private static Dictionary<Vector2Int, int> ToFoodDictionary(this IEnumerable<Food> self) {
            try {
                return self.ToDictionary(
                    it => it.Position.ToVector2Int(),
                    it => it.ExpiresIn
                );
            } catch (ArgumentException) {
                return new Dictionary<Vector2Int, int>();
            }
        }


        internal static ISimulationState ToSimulationState(this World self) =>
            new SimulationState {
                Foods = self.Food.ToFoodDictionary(),
                Worms = self.Worms.ToWormsEnumerable()
            };

        private sealed class SimulationState : ISimulationState {
            public ICollection<Vector2Int> FoodPositions => Foods.Keys;
            public IDictionary<Vector2Int, int> Foods { get; init; } = new Dictionary<Vector2Int, int>();
            public IEnumerable<Worm> Worms { get; init; } = new List<Worm>();
            public bool IsFood(Vector2Int position) => FoodPositions.Contains(position);

            public bool IsWorm(Vector2Int position) => Worms.Any(it => it.Position == position);
        }
    }
}