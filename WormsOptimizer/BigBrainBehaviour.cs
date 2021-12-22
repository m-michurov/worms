using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Worms;
using Worms.Behaviour;
using Worms.Utility;
using static Worms.Program;
using Action = Worms.Action;

namespace WormsOptimizer {
    public sealed class BigBrainBehaviour : IBehaviour {
        private const int MAX_STEPS = SIMULATION_STEPS;

        private int currentStep;
        private int minReproduceEnergy;

        private readonly ConcurrentDictionary<Vector2Int, byte> targets = new();

        public int FirstReproductionStepThreshold { get; init; }
        public int FirstReproductionEnergyThreshold { get; init; }
        public int SecondReproductionEnergyThreshold { get; init; }

        public int MaxWormsCount { get; init; } = 4;

        public BigBrainBehaviour(IReadOnlyList<int> parameters) {
            const int parametersCount = 3;

            if (parametersCount != parameters.Count) {
                throw new ArgumentException("Bad number of parameters");
            }

            var i = 0;
            FirstReproductionStepThreshold = parameters[i++];
            FirstReproductionEnergyThreshold = parameters[i++];
            SecondReproductionEnergyThreshold = parameters[i];
        }

        public BigBrainBehaviour() { }

        public Action NextAction(
            ISimulationState simulation,
            Worm worm,
            int step
        ) {
            Update(step);

            var stepsLeft = MAX_STEPS - step;
            var maxPossibleReproductions = worm.Energy / Worm.REPRODUCTION_ENERGY_COST;

            if (maxPossibleReproductions >= stepsLeft) {
                return Reproduce(simulation, worm);
            }

            if (simulation.Worms.Count() >= MaxWormsCount) {
                return GoToClosestFood(simulation, worm);
            }

            return worm.Energy >= minReproduceEnergy
                ? Reproduce(simulation, worm)
                : GoToClosestFood(simulation, worm);
        }

        private void Update(int step) {
            if (step != currentStep) {
                targets.Clear();
            }

            currentStep = step;

            minReproduceEnergy =
                currentStep < FirstReproductionStepThreshold
                    ? FirstReproductionEnergyThreshold
                    : SecondReproductionEnergyThreshold;
        }

        private IEnumerable<Vector2Int> AvailableFoodPositions(
            ISimulationState simulation,
            Vector2Int from,
            int maxDistance
        ) =>
            simulation.FoodPositions.Where(
                it => {
                    if (targets.ContainsKey(it)) {
                        return false;
                    }

                    var distance = Vector2Int.Distance(from, it);
                    return maxDistance >= distance && simulation.Foods[it] >= distance;
                }
            );

        private static IEnumerable<Direction> AvailableReproductionDirections(
            ISimulationState simulation,
            Worm worm
        ) => Direction.AllDirections.Where(it => simulation.IsEmpty(worm.Position + it));

        private Action GoToClosestFood(
            ISimulationState simulation,
            Worm worm
        ) {
            var (maybeClosestFood, _) =
                AvailableFoodPositions(
                    simulation,
                    worm.Position,
                    worm.Energy
                ).ClosestTo(worm.Position);

            var closestFood = maybeClosestFood ?? Vector2Int.Zero;
            var direction = worm.Position.DirectionTowards(closestFood, simulation.IsWorm);

            if (maybeClosestFood is not null) {
                targets[closestFood] = 0x00;
            }

            return new Action.Move(direction);
        }

        private Action Reproduce(
            ISimulationState simulation,
            Worm worm
        ) {
            var bestDirection = Direction.Down;
            var minDistance = int.MaxValue;

            foreach (var direction in AvailableReproductionDirections(simulation, worm)) {
                var position = worm.Position + direction;

                var (_, distance) =
                    AvailableFoodPositions(
                        simulation,
                        position,
                        Worm.INITIAL_ENERGY
                    ).ClosestTo(position);

                if (distance < minDistance) {
                    (bestDirection, minDistance) = (direction, distance);
                }
            }

            return new Action.Reproduce(bestDirection);
        }
    }
}