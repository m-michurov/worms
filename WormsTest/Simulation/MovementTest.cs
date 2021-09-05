using NUnit.Framework;
using Worms;
using Worms.Names;
using Worms.Utility;
using WormsTest.Behaviour;
using WormsTest.Food;
using WormsTest.StateObserver;

namespace WormsTest.Simulation {
    public sealed class MovementTest {
        [TestCase]
        public void TestMove() {
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => new Vector2Int(int.MinValue, int.MinValue)),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Move(Direction.Right)
                ),
                new DiscardObserver()
            );
            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            const int steps = Worm.INITIAL_ENERGY - 1;
            
            s.Run(steps);
            Assert.AreEqual(new Vector2Int(steps, 0), worm.Position);
        }

        [TestCase]
        public void TestMoveObstructed() {
            var s = new Worms.Simulation(
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
            var worm = s.TrySpawnWorm(initialPosition)!;
            _ = s.TrySpawnWorm(Vector2Int.UnitX);
            
            const int steps = Worm.INITIAL_ENERGY - 1;
            Assert.IsTrue(steps > 0);
            
            s.Run(steps);
            Assert.AreEqual(initialPosition, worm.Position);
        }

        [TestCase]
        public void TestMoveFood() {
            var i = 0;
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX * ++i),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Move(Direction.Right)
                ),
                new DiscardObserver()
            );

            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            Assert.AreEqual(Worm.INITIAL_ENERGY, worm.Energy);
            
            const int steps = 1;
            
            s.Run(steps);
            Assert.AreEqual(Worm.INITIAL_ENERGY + Worm.ENERGY_PER_FOOD - steps, worm.Energy);
        }
    }
}