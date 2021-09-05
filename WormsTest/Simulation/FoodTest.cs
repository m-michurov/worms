using NUnit.Framework;
using Worms;
using Worms.Food;
using Worms.Names;
using Worms.Utility;
using WormsTest.Behaviour;
using WormsTest.Food;
using WormsTest.StateObserver;

namespace WormsTest.Simulation {
    public sealed class FoodTest {
        /// <summary>
        /// For the first 10 steps food quantity should increase by 1,
        /// but after that for each added food one of already existing ones
        /// rots, keeping total food quantity at 10
        /// </summary>
        [TestCase]
        public void TestFoodDecays() {
            var s = new Worms.Simulation(
                new NameGenerator(),
                new FoodGenerator(),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Nothing()
                ),
                new DiscardObserver()
            );

            const int steps = 100;
            const int maxPossibleFood = Worms.Simulation.FOOD_LIFETIME;
            
            for (var i = 1; i <= steps; i += 1) {
                s.Run(1);

                Assert.AreEqual(
                    i < Worms.Simulation.FOOD_LIFETIME
                        ? i
                        : maxPossibleFood,
                    s.FoodPositions.Count,
                    $"{nameof(i)} = {i}"
                );
            }
        }

        [TestCase]
        public void TestFoodCreatedOnWorm() {
            var i = 0;
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX * i++),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Nothing()
                ),
                new DiscardObserver()
            );

            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            const int steps = Worm.INITIAL_ENERGY / 2;
            
            s.Run(steps);
            Assert.AreEqual(Worm.INITIAL_ENERGY - steps + Worm.ENERGY_PER_FOOD, worm.Energy);
        }

        [TestCase]
        public void TestWormCanEatTwice() {
            var i = 0;
            var s = new Worms.Simulation(
                new NameGenerator(),
                new DelegateFoodGenerator(_ => Vector2Int.UnitX + -Vector2Int.UnitX * i++),
                new DelegateBehaviour(
                    (
                        _,
                        _
                    ) => new Action.Move(Direction.Right)
                ),
                new DiscardObserver()
            );

            s.Run(1);
            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;
            Assert.AreEqual(Worm.INITIAL_ENERGY, worm.Energy);
            s.Run(1);
            Assert.AreEqual(Worm.INITIAL_ENERGY - 1 + 2 * Worm.ENERGY_PER_FOOD, worm.Energy);
        }
    }
}