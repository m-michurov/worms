using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Worms;
using Worms.StateObserver;
using Worms.Utility;
using Xunit;

namespace WormsTest.Simulation {
    public sealed class TextStateWriterTests {
        public static readonly List<object[]> States = new() {
            new object[] {
                new List<ISimulationState>(),
                new[] {""}
            },
            new object[] {
                new List<ISimulationState> {
                    new SimulationStub(Array.Empty<Worm>(), Array.Empty<Vector2Int>())
                },
                new[] {
                    "Worms: [] Food: []",
                    ""
                }
            },
            new object[] {
                new List<ISimulationState> {
                    new SimulationStub(
                        new Worm[] {new("A", new Vector2Int(1, 2))},
                        Array.Empty<Vector2Int>()
                    )
                },
                new[] {
                    "Worms: [A-10 (1, 2)] Food: []",
                    ""
                    
                }
            },
            new object[] {
                new List<ISimulationState> {
                    new SimulationStub(
                        new Worm[] {new("A", new Vector2Int(1, 2))},
                        new[] {
                            Vector2Int.UnitX, 
                            Vector2Int.UnitY
                        }
                    ),
                    new SimulationStub(
                        new Worm[] {
                            new("A", new Vector2Int(7, -3)),
                            new("B", new Vector2Int(3, 5))
                        }, 
                        new[] {
                            -Vector2Int.UnitX, 
                            Vector2Int.UnitY, 
                            Vector2Int.UnitY + Vector2Int.UnitX
                        }
                    )
                },
                new[] {
                    "Worms: [A-10 (1, 2)] Food: [(1, 0), (0, 1)]",
                    "Worms: [A-10 (7, -3), B-10 (3, 5)] Food: [(-1, 0), (0, 1), (1, 1)]",
                    ""
                }
            }
        };

        [Theory]
        [MemberData(nameof(States))]
        public void State_is_written_with_proper_formatting(
            List<ISimulationState> states_,
            string[] expected
        ) {
            // Arrange
            using var stringWriter = new StringWriter();
            var sut = new TextStateWriter(stringWriter);

            // Act
            foreach (var simulationState in states_) {
                sut.StateChanged(simulationState);
            }

            // Assert
            var result = Regex.Split(stringWriter.ToString(), "\\r\\n|\\n|\\r");
            result.Should().Equal(expected);
        }

        private sealed class SimulationStub : ISimulationState {
            private readonly IEnumerable<Worm> worms;
            private readonly IEnumerable<Vector2Int> foods;

            internal SimulationStub(
                IEnumerable<Worm> worms_,
                IEnumerable<Vector2Int> foods_
            ) => (worms, foods) = (worms_, foods_);

            public ICollection<Vector2Int> FoodPositions => Array.Empty<Vector2Int>();
            public IEnumerable<string> Foods => from food in foods select food.ToString();
            public IEnumerable<string> Worms => from worm in worms select worm.ToString();
            public bool IsFood(Vector2Int position) => false;

            public bool IsWorm(Vector2Int position) => false;
        }
    }
}