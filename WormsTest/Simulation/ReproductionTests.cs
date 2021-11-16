using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Worms;
using Worms.Names;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Simulation {
    public sealed class ReproductionTests {
        public static readonly List<object[]> Directions = new() {
            new object[] {Direction.Down},
            new object[] {Direction.Up},
            new object[] {Direction.Left},
            new object[] {Direction.Right}
        };

        [Theory]
        [MemberData(nameof(Directions))]
        public void Worm_can_reproduce_when_unobstructed_and_has_enough_energy(Direction direction) {
            const int initialEnergy = Worm.INITIAL_ENERGY + Worm.ENERGY_PER_FOOD;

            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MaxValue)),
                new DelegateBehaviour(() => new Action.Reproduce(direction)),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;
            worm.Eat();

            // Act
            sut.Run(1);

            // Assert
            worm.Energy.Should().Be(initialEnergy - Worm.ENERGY_LOSS_PER_STEP - Worm.REPRODUCTION_ENERGY_COST);
            sut.Worms.Count().Should().Be(2);
        }

        [Fact]
        public void Worm_cannot_reproduce_when_not_enough_energy() {
            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MaxValue)),
                new DelegateBehaviour(() => new Action.Reproduce(Direction.Right)),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;

            // Act
            sut.Run(1);

            // Assert
            sut.Worms.Count().Should().Be(1);
            worm.Energy.Should().Be(Worm.INITIAL_ENERGY - Worm.ENERGY_LOSS_PER_STEP);
        }

        [Fact]
        public void Worm_cannot_reproduce_when_blocked_by_another_worm() {
            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MaxValue)),
                new DelegateBehaviour(
                    (
                        _,
                        w
                    ) => {
                        if (Vector2Int.Zero == w.Position) {
                            return new Action.Reproduce(Direction.Right);
                        }

                        return new Action.Nothing();
                    }
                ),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;
            worm.Eat();
            var initialEnergy = worm.Energy;
            _ = sut.TrySpawnWorm(Vector2Int.UnitX);

            // Act
            sut.Run(1);

            // Assert
            sut.Worms.Count().Should().Be(2);
            worm.Energy.Should().Be(initialEnergy - Worm.ENERGY_LOSS_PER_STEP);
        }

        [Fact]
        public void Worm_can_reproduce_when_blocked_by_food() {
            // Arrange
            var sut = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX),
                new DelegateBehaviour(() => new Action.Reproduce(Direction.Right)),
                new DiscardObserver()
            );
            var worm = sut.TrySpawnWorm(Vector2Int.Zero)!;
            worm.Eat();
            var initialEnergy = worm.Energy;

            // Act
            sut.Run(1);

            // Assert
            sut.Worms.Count().Should().BeGreaterThan(1);
            worm.Energy.Should().Be(initialEnergy - Worm.ENERGY_LOSS_PER_STEP - Worm.REPRODUCTION_ENERGY_COST);
        }
    }
}