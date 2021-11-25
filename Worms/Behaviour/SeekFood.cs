using System.Collections.Generic;
using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class SeekFood : IBehaviour {
        internal static Direction FromToDirection(
            Vector2Int from,
            Vector2Int to
        ) {
            var (fromX, fromY) = from;
            var (toX, toY) = to;

            if (fromX > toX) {
                return Direction.Left;
            }

            if (fromX < toX) {
                return Direction.Right;
            }

            return fromY > toY ? Direction.Down : Direction.Up;
        }

        internal static Vector2Int ClosestFood(
            IEnumerable<Vector2Int> food,
            Vector2Int from
        ) {
            var closestFood = Vector2Int.Zero;
            var minDistance = int.MaxValue;
            
            foreach (var foodPosition in food) {
                var distance = Vector2Int.Distance(from, foodPosition);
                if (distance >= minDistance) {
                    continue;
                }

                minDistance = distance;
                closestFood = foodPosition;
            }

            return closestFood;
        }

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) {
            if (0 == simulation.FoodPositions.Count) {
                return new Action.Move(FromToDirection(worm.Position, Vector2Int.Zero));
            }

            var closestFood = ClosestFood(simulation.FoodPositions, worm.Position);

            if (closestFood != worm.Position) {
                return new Action.Move(FromToDirection(worm.Position, closestFood));
            }

            return new Action.Nothing();
        }
    }
}