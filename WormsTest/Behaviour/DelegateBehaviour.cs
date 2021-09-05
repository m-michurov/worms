using System;
using Worms;
using Worms.Behaviour;
using Action = Worms.Action;

namespace WormsTest.Behaviour {
    internal sealed class DelegateBehaviour : IBehaviour {
        private readonly Func<Worms.Simulation, Worm, Action> f;
        public DelegateBehaviour(Func<Worms.Simulation, Worm, Action> f_) => f = f_;

        public Action NextAction(
            Worms.Simulation simulation,
            Worm worm
        ) => f(simulation, worm);
    }
}