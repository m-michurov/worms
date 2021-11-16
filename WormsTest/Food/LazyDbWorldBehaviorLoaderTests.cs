using System;
using System.Collections.Generic;
using FluentAssertions;
using Worms.Database;
using Worms.Food;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Food {
    public sealed class LazyDbWorldBehaviorLoaderTests {
        [Fact]
        public void LazyDbWorldBehaviorLoader_loads_behavior_by_name_and_returns_food_positions_in_correct_order() {
            // Arrange
            var points = new[] {Vector2Int.Zero, Vector2Int.UnitX, Vector2Int.UnitY};
            var name = Guid.NewGuid().ToString();
            var repository = new DictionaryRepository();
            repository.Add(
                WorldBehaviourGenerator.GenerateNew(
                    name,
                    new ListFoodGenerator(points),
                    points.Length
                )
            );
            var sut = new LazyDbWorldBehaviorLoader(repository, name);

            // Act
            var retrieved = new List<Vector2Int>();
            for (var i = 0; i < points.Length; i += 1) {
                retrieved.Add(sut.NextFoodPosition(it => retrieved.Contains(it)));
            }

            // Assert
            retrieved.Should().BeEquivalentTo(points);
        }

        private sealed class DictionaryRepository : IWorldBehaviorsRepository {
            private readonly Dictionary<string, WorldBehavior> worldBehaviors = new();

            public WorldBehavior? GetByName(string name) =>
                worldBehaviors.TryGetValue(name, out var behavior) ? behavior : null;

            public void Add(WorldBehavior worldBehavior) => worldBehaviors[worldBehavior.Name] = worldBehavior;
        }
    }
}