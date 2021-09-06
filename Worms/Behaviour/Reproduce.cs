using System;
using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class Reproduce : IBehaviour {
        private readonly Random random = new();

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) => new Action.Reproduce(random.NextDirection());
    }
}