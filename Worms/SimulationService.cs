using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Worms.Behaviour;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

namespace Worms {
    internal sealed class SimulationService : IHostedService {
        private static readonly Vector2Int wormSpawnPosition = Vector2Int.Zero;

        private readonly Simulation simulation;

        public SimulationService(
            INameGenerator nameGenerator,
            IFoodGenerator foodGenerator,
            IBehaviour behaviour,
            IStateObserver stateObserver
        ) =>
            simulation = new Simulation(
                nameGenerator,
                foodGenerator,
                behaviour,
                stateObserver
            ).Chain(it => it.TrySpawnWorm(wormSpawnPosition));

        public Task StartAsync(CancellationToken cancellationToken) =>
            Task.Run(() => simulation.Run(Program.SIMULATION_STEPS), cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}