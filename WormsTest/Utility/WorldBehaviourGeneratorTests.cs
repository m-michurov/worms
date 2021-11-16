using System;
using FluentAssertions;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Utility {
    public sealed class WorldBehaviourGeneratorTests {
        [Fact]
        public void Behaviour_is_generated_with_given_name() {
            // Arrange
            var name = Guid.NewGuid().ToString();

            // Act
            var behaviour = WorldBehaviourGenerator.GenerateNew(
                name,
                new DelegateFoodGenerator(_ => Vector2Int.Zero),
                0
            );

            // Assert
            behaviour.Name.Should().Be(name);
        }

        [Fact]
        public void Food_positions_are_created_using_provided_generator() {
            // Arrange
            const int pointsCount = Worms.Simulation.FOOD_LIFETIME;
            var points = new Vector2Int[pointsCount];
            var i = 0;
            for (; i < pointsCount; i += 1) {
                points[i] = Vector2Int.UnitX * i;
            }

            // Act
            var behaviour = WorldBehaviourGenerator.GenerateNew(
                Guid.NewGuid().ToString(),
                new ListFoodGenerator(points),
                pointsCount
            );

            // Assert 
            behaviour.ToFoodPositions().Should().BeEquivalentTo(points);
        }

        [Fact]
        public void Positions_of_expired_food_can_be_reused() {
            // Arrange
            const int pointsCount = Worms.Simulation.FOOD_LIFETIME + 1;
            var points = new Vector2Int[pointsCount];
            var i = 0;
            for (; i < pointsCount - 1; i += 1) {
                points[i] = Vector2Int.UnitX * i;
            }

            points[i] = points[0];

            // Act
            var behaviour = WorldBehaviourGenerator.GenerateNew(
                Guid.NewGuid().ToString(),
                new ListFoodGenerator(points),
                pointsCount
            );

            // Assert 
            behaviour.ToFoodPositions().Should().BeEquivalentTo(points);
        }

        [Fact]
        public void Food_positions_cannot_overlap() {
            // Arrange
            const int pointsCount = Worms.Simulation.FOOD_LIFETIME;
            var points = new Vector2Int[pointsCount];
            var i = 0;
            for (; i < pointsCount - 1; i += 1) {
                points[i] = Vector2Int.UnitX * i;
            }

            points[i] = points[0];

            var generation = (Action) (() => WorldBehaviourGenerator.GenerateNew(
                Guid.NewGuid().ToString(),
                new ListFoodGenerator(points),
                pointsCount
            ));

            // Assert 
            generation.Should().Throw<Exception>();
        }
    }
}