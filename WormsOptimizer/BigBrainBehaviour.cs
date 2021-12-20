using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Worms;
using Worms.Behaviour;
using Worms.Utility;
using static Worms.Program;

namespace WormsOptimizer {
    public sealed class BigBrainBehaviour : IBehaviour {
        private const int MAX_STEPS = SIMULATION_STEPS;

        private int currentStep;
        private readonly ConcurrentDictionary<Vector2Int, byte> targets = new();

        public int FirstReproductionStepThreshold { get; init; }

        public int FirstReproductionEnergyThreshold { get; init; }

        public int SecondReproductionEnergyThreshold { get; init; }

        public int MaxWormsCount { get; init; }

        public Action NextAction(
            ISimulationState simulation,
            Worm worm,
            int step
        ) {
            if (step != currentStep) {
                currentStep = step;
                targets.Clear();
            }

            var stepsLeft = MAX_STEPS - step;
            var maxPossibleReproductions = worm.Energy / Worm.REPRODUCTION_ENERGY_COST;

            if (maxPossibleReproductions >= stepsLeft) {
                return Reproduce(simulation, worm);
            }

            if (simulation.Worms.Count() < MaxWormsCount) {
                if (step < FirstReproductionStepThreshold) {
                    if (worm.Energy >= FirstReproductionEnergyThreshold) {
                        return Reproduce(simulation, worm);
                    }
                } else {
                    if (worm.Energy >= SecondReproductionEnergyThreshold) {
                        return Reproduce(simulation, worm);
                    }
                }
            }

            return GoToClosestFood(simulation, worm);
        }

        private static List<Direction> EmptyAdjacentPositions(
            ISimulationState simulation,
            Worm worm
        ) => Direction.AllDirections.Where(it => simulation.IsEmpty(worm.Position + it)).ToList();

        private Action GoToClosestFood(
            ISimulationState simulation,
            Worm worm
        ) {
            // if (targets.ContainsKey(worm.Name)) {
            //     var target = targets[worm.Name];
            //     if (simulation.Foods.ContainsKey(target)) {
            //         return new Action.Move(SeekFood.FromToDirection(worm.Position, target));
            //     }
            // }

            var result = SeekFood.FindFood(
                simulation.FoodPositions.Where(
                    it => {
                        if (targets.ContainsKey(it)) {
                            return false;
                        }

                        var distance = Vector2Int.Distance(worm.Position, it);
                        return worm.Energy >= distance && simulation.Foods[it] >= distance;
                    }
                ),
                simulation,
                worm.Position
            );
            if (result is null) {
                return new Action.Move(SeekFood.FromToDirection(worm.Position, Vector2Int.Zero, simulation.IsWorm));
            }

            var (foodPosition, direction) = result.Value;

            targets[foodPosition] = 0x00;

            return new Action.Move(direction);
        }

        private Action Reproduce(
            ISimulationState simulation,
            Worm worm
        ) {
            var directions = EmptyAdjacentPositions(simulation, worm);
            if (directions.IsEmpty()) {
                return new Action.Reproduce(Direction.Down);
            }

            var bestDirection = Direction.Down;
            var minDistance = int.MaxValue;

            foreach (var direction in directions) {
                var position = worm.Position + direction;
                var distance = Vector2Int.Distance(
                    position,
                    SeekFood.ClosestFood(
                        simulation.FoodPositions.Where(
                            it => {
                                if (targets.ContainsKey(it)) {
                                    return false;
                                }

                                var d = Vector2Int.Distance(position, it);
                                return Worm.INITIAL_ENERGY >= d && simulation.Foods[it] >= d;
                            }
                        ),
                        position
                    )
                );
                if (distance < minDistance) {
                    (bestDirection, minDistance) = (direction, distance);
                }
            }

            return new Action.Reproduce(bestDirection);
        }
    }
}