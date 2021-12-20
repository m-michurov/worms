using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Worms;
using Worms.Behaviour;
using Worms.Utility;
using Xunit;
using Action = Worms.Action;

namespace WormsTest.Behaviour {
    public sealed class SeekFoodBehaviourTests {
        public static readonly List<object[]> Data = new() {
            new object[] {
                new Worm("", Vector2Int.Zero),
                new SimulationStub(new[] {Vector2Int.UnitX * 2, Vector2Int.UnitX * -3}),
                Direction.Right
            },
            new object[] {
                new Worm("", Vector2Int.Zero),
                new SimulationStub(new[] {Vector2Int.UnitY * 2, Vector2Int.UnitY * -3}),
                Direction.Up
            },
            new object[] {
                new Worm("", Vector2Int.Zero),
                new SimulationStub(new[] {Vector2Int.UnitX * -2, Vector2Int.UnitX * 3}),
                Direction.Left
            },
            new object[] {
                new Worm("", Vector2Int.Zero),
                new SimulationStub(new[] {Vector2Int.UnitY * -2, Vector2Int.UnitY * 3}),
                Direction.Down
            }
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void SeekFood_behaviour_chooses_direction_towards_closest_food(
            Worm worm,
            ISimulationState simulationState,
            Direction expectedDirection
        ) {
            // Arrange
            var sut = new SeekFood();

            // Act
            var action = sut.NextAction(simulationState, worm, 0);

            // Assert
            action.Should().Match<Action.Move>(move => (Vector2Int) move.Direction == expectedDirection);
        }

        [Fact]
        public void SeekFood_behaviour_chooses_to_stand_still_when_reached_food() {
            // Arrange
            var sut = new SeekFood();
            var worm = new Worm("", Vector2Int.Zero);
            var simulationState = new SimulationStub(new[] {Vector2Int.Zero});

            // Act
            var action = sut.NextAction(simulationState, worm, 0);

            // Assert
            action.Should().BeOfType<Action.Nothing>();
        }

        private sealed class SimulationStub : ISimulationState {
            private readonly IEnumerable<Vector2Int> foods;

            internal SimulationStub(IEnumerable<Vector2Int> foods_) => foods = foods_;

            public ICollection<Vector2Int> FoodPositions => foods.ToList();
            public IDictionary<Vector2Int, int> Foods => new Dictionary<Vector2Int, int>();
            public IEnumerable<Worm> Worms => Array.Empty<Worm>();
            public bool IsFood(Vector2Int position) => false;

            public bool IsWorm(Vector2Int position) => false;
        }
    }
}