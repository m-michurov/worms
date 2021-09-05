using System;
using System.Collections.Generic;
using NUnit.Framework;
using Worms.Food;
using Worms.Utility;

namespace WormsTest.Food {
    public sealed class FoodGeneratorTest {
        [TestCase]
        public void TestPositionUniqueness() {
            var g = new FoodGenerator();
            var generated = new HashSet<Vector2Int>();
            var isOccupied = (Predicate<Vector2Int>) (it => generated.Contains(it));
            
            // 68-95-99.7 rule
            // About 99.7 percent lie within three standard deviations
            // (μ ± 3σ), thus attempting to generate more than `count`
            // points (which is (6σ) squared) will result in dramatic
            // increase in generation time
            const int count = (int) (36 * FoodGenerator.STANDARD_DEVIATION * FoodGenerator.STANDARD_DEVIATION);

            for (var i = 0; i < count; i += 1) {
                generated.Add(g.NextFoodPosition(isOccupied));
            }
            
            Assert.AreEqual(count, generated.Count);
        }
    }
}