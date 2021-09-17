using System;
using System.Collections.Generic;
using FluentAssertions;
using Worms.Food;
using Worms.Utility;
using Xunit;

namespace WormsTest.Food {
    public sealed class FoodGeneratorTests {
        // 68-95-99.7 rule
        // About 99.7 percent lie within three standard deviations
        // (μ ± 3σ), thus attempting to generate more than `EXPECTED_COUNT`
        // points (which is (6σ) squared) will result in dramatic
        // increase in generation time
        private const int EXPECTED_COUNT = 
            (int) (36 * FoodGenerator.STANDARD_DEVIATION * FoodGenerator.STANDARD_DEVIATION);

        [Fact]
        public void Generated_positions_are_unique() {
            // Arrange
            var generated = new HashSet<Vector2Int>();
            var isOccupied = (Predicate<Vector2Int>) (it => generated.Contains(it));
            var sut = new FoodGenerator();

            // Act
            for (var i = 0; i < EXPECTED_COUNT; i += 1) {
                generated.Add(sut.NextFoodPosition(isOccupied));
            }
            
            // Assert
            generated.Count.Should().Be(EXPECTED_COUNT);
        }
    }
}