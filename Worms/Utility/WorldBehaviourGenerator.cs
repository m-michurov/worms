using System.Collections.Generic;
using Worms.Database;
using Worms.Food;

namespace Worms.Utility {
    internal static class WorldBehaviourGenerator {
        internal static WorldBehavior GenerateNew(
            string name,
            IFoodGenerator foodGenerator,
            int steps
        ) {
            var foodLifetime = new Dictionary<Vector2Int, int>();
            var positions = new List<FoodPosition>();

            for (var i = 0; i < steps; i += 1) {
                foodLifetime.RemoveWhere(entry => entry.Value <= 0);
                var position = foodGenerator.NextFoodPosition(p => foodLifetime.ContainsKey(p));
                foodLifetime[position] = Simulation.FOOD_LIFETIME;
                positions.Add(
                    new FoodPosition {
                        Step = i + 1,
                        X = position.X,
                        Y = position.Y
                    }
                );

                foreach (var p in foodLifetime.Keys) {
                    foodLifetime[p] -= Simulation.FOOD_DECAY_RATE;
                }
            }

            return new WorldBehavior {
                Name = name,
                FoodPositions = positions
            };
        }
    }
}