using System;
using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class SeekFood : IBehaviour {
        private readonly Random random = new();

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) {
            if (0 == simulation.FoodPositions.Count) {
                return new Action.Move(random.NextDirection());
            }

            var closest = Vector2Int.Zero;
            var minDistance = int.MaxValue;
            foreach (var foodPosition in simulation.FoodPositions) {
                var distance = Vector2Int.Distance(worm.Position, foodPosition);
                if (distance >= minDistance) {
                    continue;
                }

                minDistance = distance;
                closest = foodPosition;
            }

            var (fX, fY) = closest;
            var (x, y) = worm.Position;

            if (fX > x) {
                return new Action.Move(Direction.Right);
            }

            if (fX < x) {
                return new Action.Move(Direction.Left);
            }

            if (fY > y) {
                return new Action.Move(Direction.Up);
            }

            if (fY < y) {
                return new Action.Move(Direction.Down);
            }

            return new Action.Nothing();
        }
    }
}