using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Worms.Behaviour;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

namespace Worms {
    internal sealed class HostedSimulation : BackgroundService {
        private const int STEPS = 100;
        private static readonly Vector2Int wormSpawnPosition = Vector2Int.Zero;

        private readonly Simulation simulation;

        public HostedSimulation(
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

        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            Task.Run(() => simulation.Run(STEPS), stoppingToken);
    }
}