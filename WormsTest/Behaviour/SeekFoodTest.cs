using System.Linq;
using NUnit.Framework;
using Worms;
using Worms.Behaviour;
using Worms.Names;
using Worms.Utility;
using WormsTest.StateObserver;
using WormsTest.TestImplementations;

namespace WormsTest.Behaviour {
    public sealed class SeekFoodTest {
        /// <summary>
        /// Given a worm, let `shortestDistance` = distance between the
        /// worm and closest food position.
        /// Test that worm eats within `shortestDistance` steps,
        /// which means the worm took the shortest path to the food closest food.
        /// </summary>
        [TestCase]
        public void TestFindsClosestFood() {
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
            Assert.AreEqual(foods.Length, s.FoodPositions.Count);

            var worm = s.TrySpawnWorm(Vector2Int.Zero)!;

            var shortestDistance =
                foods
                    .Select(food => Vector2Int.Distance(food, worm.Position))
                    .Prepend(int.MaxValue)
                    .Min();

            Assert.AreEqual(Worm.INITIAL_ENERGY, worm.Energy);

            s.Run(shortestDistance);

            Assert.AreEqual(
                Worm.INITIAL_ENERGY - shortestDistance * Worm.ENERGY_LOSS_PER_STEP + Worm.ENERGY_PER_FOOD,
                worm.Energy
            );
        }
    }
}