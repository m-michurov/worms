using System;
using Worms;
using Worms.Behaviour;
using Action = Worms.Action;

namespace WormsTest.TestImplementations {
    internal sealed class DelegateBehaviour : IBehaviour {
        private readonly Func<ISimulationState, Worm, Action> f;
        public DelegateBehaviour(Func<ISimulationState, Worm, Action> f_) => f = f_;
        
        public DelegateBehaviour(Func<Action> f_) => f = (_, _) => f_();

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) => f(simulation, worm);
    }
}