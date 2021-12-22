using System;
using System.Collections.Generic;

namespace Worms.Utility {
    public static class PathfindingUtility {
        public static Direction DirectionTowards(
            this Vector2Int from,
            Vector2Int to,
            Func<Vector2Int, bool> isOccupied
        ) {
            var (fromX, fromY) = from;
            var (toX, toY) = to;

            if (fromX > toX && false == isOccupied(from + Direction.Left)) {
                return Direction.Left;
            }

            if (fromX < toX && false == isOccupied(from + Direction.Right)) {
                return Direction.Right;
            }

            return fromY > toY && false == isOccupied(from + Direction.Down) ? Direction.Down : Direction.Up;
        }

        public static Direction DirectionTowards(
            this Vector2Int from,
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

        public static (Vector2Int?, int) ClosestTo(
            this IEnumerable<Vector2Int> points,
            Vector2Int target
        ) {
            var (closest, minDistance) = (null as Vector2Int?, int.MaxValue);

            foreach (var foodPosition in points) {
                var distance = Vector2Int.Distance(target, foodPosition);
                if (distance < minDistance) {
                    (closest, minDistance) = (foodPosition, distance);
                }
            }

            return (closest, minDistance);
        }
    }
}