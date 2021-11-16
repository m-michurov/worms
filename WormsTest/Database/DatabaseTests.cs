using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Worms.Database;
using Worms.Utility;
using WormsTest.TestImplementations;
using Xunit;

namespace WormsTest.Database {
    public sealed class DatabaseTests {
        internal static MainContext CreateInMemoryDb() =>
            new(new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        [Fact]
        public void Behaviour_is_saved_with_given_name() {
            // Arrange
            var name = Guid.NewGuid().ToString();
            var points = new[] {Vector2Int.Zero, Vector2Int.UnitX, Vector2Int.UnitY};

            var behaviour = WorldBehaviourGenerator.GenerateNew(
                name,
                new ListFoodGenerator(points),
                points.Length
            );

            using var db = CreateInMemoryDb();
            var sut = new WorldBehaviorsRepository(db);

            // Act
            sut.Add(behaviour);

            // Assert
            var saved = sut.GetByName(name);
            saved.Should().NotBeNull();
            saved.Should().BeEquivalentTo(behaviour);
        }
    }
}