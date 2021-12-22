using System;
using System.Linq;
using Worms;
using Worms.Behaviour;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;
using static Worms.Program;

namespace WormsOptimizer {
    internal static class Program {
        private const int RUNS = 100;

        private static void Main() {
            GridSearch(
                new[] {40, 20, 50},
                new[] {0, 1, 2}
            );
            GridSearch(
                new[] {40, 20, 50},
                new[] {0, 2, 1}
            );
            GridSearch(
                new[] {40, 20, 50},
                new[] {1, 0, 2}
            );
            GridSearch(
                new[] {40, 20, 50},
                new[] {1, 2, 0}
            );
            GridSearch(
                new[] {40, 20, 50},
                new[] {2, 0, 1}
            );
            GridSearch(
                new[] {40, 20, 50},
                new[] {2, 1, 0}
            );
        }

        private static void GridSearch(
            int[] initialValues,
            int[] indicesOrder,
            float initialDelta = 20f
        ) {
            // Console.WriteLine($"Indexing order: {string.Join(", ", indicesOrder)}");
            var bestScore = 0.0f;
            var bestParams = new int[initialValues.Length];
            var parameters = new int[initialValues.Length];

            Array.Copy(initialValues, parameters, initialValues.Length);

            foreach (var parameterIndex in indicesOrder) {
                parameters[parameterIndex] = initialValues[parameterIndex];

                var score = Score(
                    () => new BigBrainBehaviour(parameters),
                    RUNS * 3
                );

                var delta = initialDelta * 2;

                while (delta > 1f) {
                    var initialValue = parameters[parameterIndex];

                    parameters[parameterIndex] = initialValue + (int) delta;
                    var scoreMore = Score(() => new BigBrainBehaviour(parameters));

                    Console.Write(
                        $"\r[{parameterIndex}] Value: {parameters[parameterIndex]} " +
                        $"(Best: {initialValue}) Score: {scoreMore:f2} (Best: {score:f2})" +
                        "                                                              "
                    );

                    if (scoreMore > score) {
                        score = scoreMore;
                        continue;
                    }

                    parameters[parameterIndex] = initialValue - (int) delta;
                    var scoreLess = Score(() => new BigBrainBehaviour(parameters));

                    Console.Write(
                        $"\r[{parameterIndex}] Value: {parameters[parameterIndex]} " +
                        $"(Best: {initialValue}) Score: {scoreLess:f2} (Best: {score:f2})" +
                        "                                                              "
                    );

                    if (scoreLess > score) {
                        score = scoreLess;
                        continue;
                    }

                    delta /= 2;
                    parameters[parameterIndex] = initialValue;
                }

                Console.Write(
                    "\r                                                                   " +
                    "                                                                   \r"
                );

                if (score <= bestScore) {
                    continue;
                }

                bestScore = score;
                Array.Copy(parameters, bestParams, parameters.Length);
            }

            Console.WriteLine($"Best score: {bestScore:f2}");
            Console.WriteLine($"Best parameters: {string.Join(", ", bestParams)}\n");
        }

        private static float Score(
            Func<IBehaviour> behaviour,
            int runs = RUNS * 10
        ) {
            var random = new Random(Seed: 444);

            var totalWorms = 0f;
            for (var run = 0;
                run < runs;
                run += 1) {
                var simulation = CreateSimulation(behaviour(), random);
                simulation.Run(SIMULATION_STEPS);

                var worms = simulation.Worms.ToList();
                totalWorms += worms.Count;
            }

            return totalWorms / runs;
        }

        private static Simulation CreateSimulation(
            IBehaviour behaviour,
            Random random
        ) =>
            new Simulation(
                new NameGenerator(),
                new RandomFoodGenerator(random),
                behaviour,
                new DiscardObserver()
            ).Chain(it => it.TrySpawnWorm(Vector2Int.Zero));

        private sealed class DiscardObserver : IStateObserver {
            public void StateChanged(ISimulationState _) { }
        }
    }
}