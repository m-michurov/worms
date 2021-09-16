using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Worms.Names;

namespace WormsTest.Names {
    public sealed class NameGeneratorTests {
        private const int A_LOT = 1_000_000;

        [Fact]
        public void Generated_names_are_unique() {
            // Arrange
            var generated = new HashSet<string>();
            var sut = new NameGenerator();

            // Act
            for (var i = 0; i < A_LOT; i += 1) {
                generated.Add(sut.NextName);
            }

            // Assert
            generated.Count.Should().Be(A_LOT);
        }
    }
}