using Worms;
using Worms.StateObserver;

namespace WormsTest.TestImplementations {
    internal sealed class DiscardObserver : IStateObserver {
        public void StateChanged(ISimulationState simulation) { }
    }
}