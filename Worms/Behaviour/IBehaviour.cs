namespace Worms.Behaviour {
    internal interface IBehaviour {
        Action NextAction(
            ISimulationState simulation,
            Worm worm
        );
    }
}