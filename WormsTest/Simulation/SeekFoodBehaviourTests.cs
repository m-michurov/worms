using System.Linq;
using FluentAssertions;
using Worms;
using Worms.Behaviour;
using Worms.Names;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Simulation {
    public sealed class SeekFoodBehaviourTests {
        /// <summary>
        ///     Given a worm, let `shortestDistance` = distance between the
        ///     worm and closest food position.
        ///     Test that worm eats within `shortestDistance` steps,
        ///     which means the worm took the shortest path to the food closest food.
        /// </summary>
        [Fact]
        public void Worm_follows_shortest_path_to_closest_food() {
            // Arrange
            var sut = CreateTestSimulation();
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;
            var shortestDistance =
                sut.FoodPositions
                    .Select(food => Vector2Int.Distance(food, worm.Position))
                    .Prepend(int.MaxValue)
                    .Min();

            // Act
            sut.Run(shortestDistance);

            // Assert
            var expectedEnergy =
                Worm.INITIAL_ENERGY
                - Worm.ENERGY_LOSS_PER_STEP * shortestDistance
                + Worm.ENERGY_PER_FOOD;

            worm.Energy.Should().Be(expectedEnergy);
        }

        private static Worms.Simulation CreateTestSimulation() {
            var foods = new Vector2Int[] {
                new(3, 2),
                new(-2, 1),
                new(2, -3),
                new(-1, -3)
            };
            var foodIndex = 0;
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(
                    _ => {
                        // Food position is never repeated
                        var pos = foods[foodIndex % foods.Length] * (1 + foodIndex / foods.Length);
                        foodIndex += 1;
                        return pos;
                    }
                ),
                new SeekFood(),
                new DiscardObserver()
            );

            s.Run(foods.Length);

            return s;
        }
    }
}