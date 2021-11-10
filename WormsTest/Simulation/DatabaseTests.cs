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
        private const int STEPS = 100;

        private static WorldBehaviorContext CreateInMemoryDb() => Database.DatabaseTests.CreateInMemoryDb();

        /// <summary>
        /// Added behaviour creates food in a line along X axis
        /// </summary>
        private static WorldBehaviorsRepository PopulateWithBehaviour(
            WorldBehaviorContext db,
            string name
        ) {
            var points = new Vector2Int[STEPS];

            for (var i = 0; i < STEPS; i += 1) {
                points[i] = Vector2Int.UnitX * i;
            }

            var foodGenerator = new ListFoodGenerator(points);

            var repository = new WorldBehaviorsRepository(db);

            var behavior = WorldBehaviourGenerator.GenerateNew(
                name,
                foodGenerator,
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

            // Act
            var behaviour = repository.GetByName(name);
            var foodGenerator = new ListFoodGenerator(behaviour!.ToFoodPositions()) as IFoodGenerator;
            var simulation = new Worms.Simulation(
                new NameGenerator(),
                foodGenerator,
                new SeekFood(),
                new DiscardObserver()
            );
            var worm = simulation.TrySpawnWorm(Vector2Int.Zero)!;

            simulation.Run(STEPS);

            // Assert
            const int expectedEnergy =
                Worm.INITIAL_ENERGY
                - Worm.ENERGY_LOSS_PER_STEP * STEPS
                + Worm.ENERGY_PER_FOOD * (STEPS - 1);
            
            worm.Energy.Should().Be(expectedEnergy);

            simulation.Worms.Count().Should().Be(1);
        }
    }
}