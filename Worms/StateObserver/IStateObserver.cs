namespace Worms.StateObserver {
    internal interface IStateObserver {
        void StateChanged(ISimulationState simulation);
    }
}