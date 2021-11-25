using System;

namespace Worms.Behaviour {
    internal sealed class Reproduce : IBehaviour {
        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) => new Action.Reproduce(
            SeekFood.FromToDirection(
                worm.Position,
                SeekFood.ClosestFood(simulation.FoodPositions, worm.Position)
            )
        );
    }
}