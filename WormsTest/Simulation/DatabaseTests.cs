using System;
using System.Linq;
using FluentAssertions;
using Worms;
using Worms.Behaviour;
using Worms.Database;
using Worms.Food;
using Worms.Names;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Simulation {
    public sealed class DatabaseTests {
        private const int STEPS = Program.SIMULATION_STEPS;

        private static MainContext CreateInMemoryDb() => Database.DatabaseTests.CreateInMemoryDb();

        /// <summary>
        ///     Added behaviour creates food in a line along X axis
        /// </summary>
        private static WorldBehaviorsRepository PopulateWithBehaviour(
            MainContext db,
            string name
        ) {
            var points = new Vector2Int[STEPS];

            for (var i = 0; i < STEPS; i += 1) {
                points[i] = Vector2Int.UnitX * i;
            }

            var repository = new WorldBehaviorsRepository(db);

            var behavior = WorldBehaviourGenerator.GenerateNew(
                name,
                new ListFoodGenerator(points),
                STEPS
            );

            repository.Add(behavior);

            return repository;
        }

        [Fact]
        public void Worms_live_in_pre_made_world() {
            // Arrange
            using var db = CreateInMemoryDb();
            var name = Guid.NewGuid().ToString();
            var repository = PopulateWithBehaviour(db, name);
            var simulation = new Worms.Simulation(
                new NameGenerator(),
                new LazyDbWorldBehaviorLoader(repository, name),
                new SeekFood(),
                new DiscardObserver()
            );
            var worm = simulation.TrySpawnWorm(Vector2Int.Zero)!;

            // Act
            simulation.Run(STEPS);

            // Assert
            const int expectedEnergy =
                Worm.INITIAL_ENERGY
                - Worm.ENERGY_LOSS_PER_STEP * STEPS
                + Worm.ENERGY_PER_FOOD * (STEPS - 1);

            worm.Energy.Should().Be(expectedEnergy);

            simulation.Worms.Count().Should().Be(1);
        }

        [Fact]
        public void Multiple_executions_yield_same_results() {
            static ISimulationState RunSimulation(
                IWorldBehaviorsRepository repository_,
                string name_
            ) => new Worms.Simulation(
                    new NameGenerator(new Random(0)),
                    new LazyDbWorldBehaviorLoader(repository_, name_),
                    new SeekFood(),
                    new DiscardObserver()
                )
                .Chain(it => it.TrySpawnWorm(Vector2Int.Zero))
                .Chain(it => it.Run(STEPS));

            // Arrange
            using var db = CreateInMemoryDb();
            var name = Guid.NewGuid().ToString();
            var repository = PopulateWithBehaviour(db, name);

            // Act
            var state1 = RunSimulation(repository, name);
            var state2 = RunSimulation(repository, name);

            // Assert
            state1.Should().BeEquivalentTo(state2);
        }
    }
}