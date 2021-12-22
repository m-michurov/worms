using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class Reproduce : IBehaviour {
        public Action NextAction(
            ISimulationState simulation,
            Worm worm,
            int _
        ) {
            var (closestFood, _) = simulation.FoodPositions.ClosestTo(worm.Position);
            return new Action.Reproduce(worm.Position.DirectionTowards(closestFood ?? Vector2Int.Zero));
        }
    }
}