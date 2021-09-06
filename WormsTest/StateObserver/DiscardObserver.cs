using Worms;
using Worms.StateObserver;

namespace WormsTest.StateObserver {
    internal sealed class DiscardObserver : IStateObserver {
        public void StateChanged(ISimulationState simulation) { }
    }
}