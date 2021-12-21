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
                targets.Clear();
            } else {
                targets.RemoveWhere(it => false == simulation.IsFood(it.Key));
            }

            currentStep = step;

            var stepsLeft = MAX_STEPS - step;
            var maxPossibleReproductions = worm.Energy / Worm.REPRODUCTION_ENERGY_COST;

            if (maxPossibleReproductions >= stepsLeft) {
                return Reproduce(simulation, worm);
            }

            if (simulation.Worms.Count() >= MaxWormsCount) {
                return GoToClosestFood(simulation, worm);
            }

            if (step < FirstReproductionStepThreshold) {
                if (worm.Energy >= FirstReproductionEnergyThreshold) {
                    return Reproduce(simulation, worm);
                }
            } else {
                if (worm.Energy >= SecondReproductionEnergyThreshold) {
                    return Reproduce(simulation, worm);
                }
            }

            return GoToClosestFood(simulation, worm);
        }

        private static IEnumerable<Direction> EmptyAdjacentPositions(
            ISimulationState simulation,
            Worm worm
        ) => Direction.AllDirections.Where(it => simulation.IsEmpty(worm.Position + it));

        private Action GoToClosestFood(
            ISimulationState simulation,
            Worm worm
        ) {
            var availableFood =
                simulation.FoodPositions.Where(it => false == targets.ContainsKey(it));

            var result = SeekFood.FindFood(
                availableFood.Where(
                    it => {
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
            var bestDirection = Direction.Down;
            var minDistance = int.MaxValue;
            
            var availableFood =
                simulation.FoodPositions.Where(it => false == targets.ContainsKey(it)).ToList();
            
            foreach (var direction in directions) {
                var position = worm.Position + direction;
                var food = SeekFood.ClosestFood(
                    availableFood.Where(
                        it => {
                            var d = Vector2Int.Distance(position, it);
                            return Worm.INITIAL_ENERGY >= d && simulation.Foods[it] >= d;
                        }
                    ),
                    position
                );
                var distance = Vector2Int.Distance(position, food);
                if (distance < minDistance) {
                    (bestDirection, minDistance) = (direction, distance);
                }
            }

            return new Action.Reproduce(bestDirection);
        }
    }
}