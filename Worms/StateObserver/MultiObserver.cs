namespace Worms.StateObserver {
    internal sealed class MultiObserver : IStateObserver {
        private readonly IStateObserver[] observers;
        
        public MultiObserver(params IStateObserver[] observers_) => observers = observers_;

        public void StateChanged(ISimulationState simulation) {
            foreach (var observer in observers) {
                observer.StateChanged(simulation);
            }
        }
    }
}