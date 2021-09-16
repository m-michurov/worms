using FluentAssertions;
using Worms;
using Worms.Food;
using Worms.Names;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Simulation {
    public sealed class FoodTests {
        /// <summary>
        /// For the first `Worms.Simulation.FOOD_LIFETIME` steps food
        /// quantity should increase by 1, but after that for each added
        /// food one of already existing ones rots, keeping total food
        /// quantity at `Worms.Simulation.FOOD_LIFETIME`
        /// </summary>
        [Fact]
        public void Food_decays_over_time() {
            const int maxSteps = 100;
            const int maxPossibleFood = Worms.Simulation.FOOD_LIFETIME;

            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new FoodGenerator(),
                new DelegateBehaviour(() => new Action.Nothing()),
                new DiscardObserver()
            );
            
            for (var steps = 1; steps <= maxPossibleFood; steps += 1) {
                // Act
                sut.Run(1);

                // Assert
                sut.FoodPositions.Count.Should().Be(steps);
            }

            for (var i = maxPossibleFood + 1; i <= maxSteps; i += 1) {
                // Act
                sut.Run(1);

                // Assert
                sut.FoodPositions.Count.Should().Be(maxPossibleFood);
            }
        }

        [Fact]
        public void Food_is_consumed_if_created_on_worm_position() {
            const int steps = Worm.INITIAL_ENERGY / 2;

            // Arrange
            var i = 0;
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX * i++),
                new DelegateBehaviour(() => new Action.Nothing()),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;

            // Act
            sut.Run(steps);

            // Assert
            const int expectedEnergy =
                Worm.INITIAL_ENERGY
                - Worm.ENERGY_LOSS_PER_STEP * steps
                + Worm.ENERGY_PER_FOOD;
            
            worm.Energy.Should().Be(expectedEnergy);
        }

        [Fact]
        public void Worm_can_eat_twice_per_step() {
            // Arrange
            var i = 0;
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX + -Vector2Int.UnitX * i++),
                new DelegateBehaviour(() => new Action.Move(Direction.Right)),
                new DiscardObserver()
            );
            s.Run(1);
            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            
            // Act
            s.Run(1);
            
            // Assert
            const int expectedEnergy =
                Worm.INITIAL_ENERGY 
                - Worm.ENERGY_LOSS_PER_STEP 
                + 2 * Worm.ENERGY_PER_FOOD;
            
            worm.Energy.Should().Be(expectedEnergy);
        }
    }
}