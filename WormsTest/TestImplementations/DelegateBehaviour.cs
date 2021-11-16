using Worms;
using Worms.Behaviour;

namespace WormsTest.TestImplementations {
    internal sealed class DelegateBehaviour : IBehaviour {
        private readonly System.Func<ISimulationState, Worm, Action> f;
        public DelegateBehaviour(System.Func<ISimulationState, Worm, Action> f_) => f = f_;

        public DelegateBehaviour(System.Func<Action> f_) => f = (_, _) => f_();

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) => f(simulation, worm);
    }
}