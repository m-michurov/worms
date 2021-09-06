using System;
using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class HedonisticBehaviour : IBehaviour {
        private const double REPRODUCE_PROBABILITY = .15;

        private readonly Reproduce reproduce = new();
        private readonly SeekFood seekFood = new();

        private readonly Random random = new();

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) => random.NextBool(REPRODUCE_PROBABILITY)
            ? reproduce.NextAction(simulation, worm)
            : seekFood.NextAction(simulation, worm);
    }
}