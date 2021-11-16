using System;
using System.Collections.Generic;
using FluentAssertions;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Food {
    // Testing stubs because why not
    public sealed class ListFoodGeneratorTests {
        [Fact]
        public void ListFoodGenerator_cannot_return_occupied_positions() {
            // Arrange
            var sut = new ListFoodGenerator(new[] {Vector2Int.Zero});

            var op = (Action) (() => sut.NextFoodPosition(_ => true));

            // Assert
            op.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ListFoodGenerator_returns_provided_positions_in_correct_order() {
            // Arrange
            var points = new[] {Vector2Int.Zero, Vector2Int.UnitX, Vector2Int.UnitY};
            var sut = new ListFoodGenerator(points);
            var retrieved = new List<Vector2Int>();

            // Act
            for (var i = 0; i < points.Length; i += 1) {
                retrieved.Add(sut.NextFoodPosition(point => retrieved.Contains(point)));
            }

            // Assert
            retrieved.Should().BeEquivalentTo(points);
        }
    }
}