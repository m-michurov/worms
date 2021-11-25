namespace Worms.Behaviour {
    internal sealed class HedonisticBehaviour : IBehaviour {
        private readonly Reproduce reproduce = new();
        private readonly SeekFood seekFood = new();

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) => worm.Energy > Worm.REPRODUCTION_ENERGY_COST * 3 / 2
            ? reproduce.NextAction(simulation, worm)
            : seekFood.NextAction(simulation, worm);
    }
}