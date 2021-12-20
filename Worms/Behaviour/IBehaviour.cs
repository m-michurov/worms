namespace Worms.Behaviour {
    public interface IBehaviour {
        Action NextAction(
            ISimulationState simulation,
            Worm worm,
            int step
        );
    }
}