using System.Linq;
using NUnit.Framework;
using Worms;
using Worms.Names;
using Worms.Utility;
using WormsTest.StateObserver;
using WormsTest.TestImplementations;

namespace WormsTest.Simulation {
    public sealed class ReproductionTest {
        [TestCase]
        public void TestReproduction() {
            static void TestForDirection(Direction direction) {
                var s = new Worms.Simulation(
                    new NameGenerator(),
                    new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MaxValue)),
                    new DelegateBehaviour(
                        (
                            _,
                            _
                        ) => new Action.Reproduce(direction)
                    ),
                    new DiscardObserver()
                );

                var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
                worm.Eat();

                const int initialEnergy = Worm.INITIAL_ENERGY + Worm.ENERGY_PER_FOOD;
                Assert.AreEqual(initialEnergy, worm.Energy);
                Assert.AreEqual(1, s.Worms.Count());

                s.Run(1);

                Assert.AreEqual(
                    initialEnergy - Worm.ENERGY_LOSS_PER_STEP - Worm.REPRODUCTION_ENERGY_COST,
                    worm.Energy
                );
                Assert.AreEqual(2, s.Worms.Count());
            }

            Assert.DoesNotThrow(() => TestForDirection(Direction.Right));
            Assert.DoesNotThrow(() => TestForDirection(Direction.Left));
            Assert.DoesNotThrow(() => TestForDirection(Direction.Up));
            Assert.DoesNotThrow(() => TestForDirection(Direction.Down));
        }

        [TestCase]
        public void TestReproduceNotEnoughEnergy() {
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MaxValue)),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Reproduce(Direction.Right)
                ),
                new DiscardObserver()
            );
            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            const int initialWormsCount = 1;

            Assert.AreEqual(initialWormsCount, s.Worms.Count());
            Assert.AreEqual(Worm.INITIAL_ENERGY, worm.Energy);

            s.Run(1);

            Assert.AreEqual(initialWormsCount, s.Worms.Count());
            Assert.AreEqual(Worm.INITIAL_ENERGY - Worm.ENERGY_LOSS_PER_STEP, worm.Energy);
        }

        [TestCase]
        public void TestReproduceObstructedByWorms() {
            var s = new Worms.Simulation(
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
            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            worm.Eat();

            var initialEnergy = worm.Energy;
            // Worm has enough energy to reproduce
            Assert.IsTrue(initialEnergy > Worm.REPRODUCTION_ENERGY_COST);
            _ = s.TrySpawnWorm(Vector2Int.UnitX);

            const int initialWormsCount = 2;

            Assert.AreEqual(initialWormsCount, s.Worms.Count());

            s.Run(1);

            // No new worms have been created
            Assert.AreEqual(initialWormsCount, s.Worms.Count());
            // Reproduction attempt costs no energy 
            Assert.AreEqual(initialEnergy - Worm.ENERGY_LOSS_PER_STEP, worm.Energy);
        }

        [TestCase]
        public void TestReproduceObstructedByFood() {
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Reproduce(Direction.Right)
                ),
                new DiscardObserver()
            );
            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            worm.Eat();

            var initialEnergy = worm.Energy;
            // Worm has enough energy to reproduce
            Assert.IsTrue(initialEnergy > Worm.REPRODUCTION_ENERGY_COST);

            const int initialWormsCount = 1;

            Assert.AreEqual(initialWormsCount, s.Worms.Count());

            s.Run(1);

            // No new worms have been created
            Assert.AreEqual(initialWormsCount, s.Worms.Count());
            // Reproduction attempt costs no energy 
            Assert.AreEqual(initialEnergy - Worm.ENERGY_LOSS_PER_STEP, worm.Energy);
        }
    }
}