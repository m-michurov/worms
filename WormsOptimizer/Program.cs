using System;
using System.Collections.Generic;
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
            var (bestParams, bestScore) = GridSearch(
                new[] {30, 32, 33, 34},
                new[] {19, 20, 21},
                new[] {54, 82, 80},
                new[] {4}
            );

            Console.WriteLine($"Score: {bestScore:f3}");
            Console.WriteLine($"Best parameters: {bestParams}");
        }

        private static ((int, int, int, int), float) GridSearch(
            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            IReadOnlyCollection<int> firstReproductionStepThreshold,
            IReadOnlyCollection<int> firstReproductionEnergyThreshold,
            IReadOnlyCollection<int> secondReproductionEnergyThreshold,
            IReadOnlyCollection<int> maxWormsCount
        ) {
            var bestScore = 0.0f;
            var bestParams = (0, 0, 0, 0);

            var totalCombinations = firstReproductionStepThreshold.Count
                                    * firstReproductionEnergyThreshold.Count
                                    * secondReproductionEnergyThreshold.Count
                                    * maxWormsCount.Count;
            var combinations = 0;

            Console.WriteLine($"Total: {totalCombinations}");

            foreach (var i1 in firstReproductionStepThreshold) {
                foreach (var i2 in firstReproductionEnergyThreshold) {
                    foreach (var i3 in secondReproductionEnergyThreshold) {
                        foreach (var i4 in maxWormsCount) {
                            var score = CollectStatistics(
                                () => new BigBrainBehaviour {
                                    FirstReproductionStepThreshold = i1,
                                    FirstReproductionEnergyThreshold = i2,
                                    SecondReproductionEnergyThreshold = i3,
                                    MaxWormsCount = i4
                                },
                                RUNS * 3
                            );

                            combinations += 1;
                            Console.Write($"\r{combinations}/{totalCombinations} Best score: {bestScore:f3}");

                            if (score <= bestScore) {
                                continue;
                            }

                            bestScore = score;
                            bestParams = (i1, i2, i3, i4);
                        }
                    }
                }
            }

            Console.WriteLine();

            return (bestParams, bestScore);
        }

        private static float CollectStatistics(
            Func<IBehaviour> behaviour,
            int runs
        ) {
            var random = new Random(Seed: 596);
            var totalWorms = 0f;

            for (var run = 0; run < runs; run += 1) {
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