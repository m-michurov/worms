using NUnit.Framework;
using Worms;
using Worms.Food;
using Worms.Names;
using WormsTest.Behaviour;
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
    }
}