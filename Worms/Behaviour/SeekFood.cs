using System;
using System.Collections.Generic;
using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class SeekFood : IBehaviour {
        internal static Direction FromToDirection(
            Vector2Int from,
            Vector2Int to,
            Func<Vector2Int, bool>? isOccupied = null
        ) {
            var (fromX, fromY) = from;
            var (toX, toY) = to;

            isOccupied ??= _ => false;

            if (fromX > toX && false == isOccupied(from + Direction.Left)) {
                return Direction.Left;
            }
            
            if (fromX < toX && false == isOccupied(from + Direction.Right)) {
                return Direction.Right;
            }

            return fromY > toY && false == isOccupied(from + Direction.Down) ? Direction.Down : Direction.Up;
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

        public static (Vector2Int, Direction)? FindFood(
            IEnumerable<Vector2Int> foodPositions,
            Vector2Int from
        ) {
            var closestFood = ClosestFood(foodPositions, from);
            return (closestFood, FromToDirection(from, closestFood));
        }

        public static (Vector2Int, Direction)? FindFood(
            IEnumerable<Vector2Int> foodPositions,
            ISimulationState simulation,
            Vector2Int from
        ) {
            var closestFood = ClosestFood(foodPositions, from);
            return (closestFood, FromToDirection(from, closestFood, simulation.IsWorm));
        }

        public Action NextAction(
            ISimulationState simulation,
            Worm worm,
            int _
        ) {
            var result = FindFood(simulation.FoodPositions, worm.Position);
            if (result is null) {
                return new Action.Nothing();
            }

            var (_, direction) = result.Value;

            return new Action.Move(direction);
        }
    }
}