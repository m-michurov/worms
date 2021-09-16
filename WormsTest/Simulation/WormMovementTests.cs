using FluentAssertions;
using Worms;
using Worms.Names;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Simulation {
    public sealed class WormMovementTests {
        [Fact]
        public void Worm_can_move_when_unobstructed() {
            const int steps = Worm.INITIAL_ENERGY - 1;

            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MinValue)),
                new DelegateBehaviour(() => new Action.Move(Direction.Right)),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;

            // Act
            sut.Run(steps);

            // Assert
            worm.Position.Should().Be(Vector2Int.UnitX * steps);
        }

        [Fact]
        public void Worm_cannot_move_when_blocked_by_another_worm() {
            const int steps = Worm.INITIAL_ENERGY - 1;

            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MinValue)),
                new DelegateBehaviour(
                    (
                        _,
                        w
                    ) => {
                        if (Vector2Int.Zero == w.Position) {
                            return new Action.Move(Direction.Right);
                        }

                        return new Action.Nothing();
                    }
                ),
                new DiscardObserver()
            );
            var initialPosition = Vector2Int.Zero;
            var worm = sut.TrySpawnWorm(initialPosition)!;
            _ = sut.TrySpawnWorm(Vector2Int.UnitX);

            // Act
            sut.Run(steps);

            // Assert
            worm.Position.Should().Be(initialPosition);
        }

        [Fact]
        public void Worm_can_eat_food_by_moving_to_foods_position() {
            const int steps = 1;

            // Arrange
            var i = 0;
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX * ++i),
                new DelegateBehaviour(() => new Action.Move(Direction.Right)),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;

            // Act
            sut.Run(steps);

            // Assert
            const int expectedEnergy =
                Worm.INITIAL_ENERGY
                - steps * Worm.ENERGY_LOSS_PER_STEP
                + Worm.ENERGY_PER_FOOD;

            worm.Energy.Should().Be(expectedEnergy);
            worm.Position.Should().Be(Vector2Int.UnitX * steps);
        }
    }
}