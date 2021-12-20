using System;
using System.Collections.Generic;
using System.Linq;
using Worms.Behaviour;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

namespace Worms {
    internal sealed class Simulation : ISimulationState {
        internal const int FOOD_DECAY_RATE = 1;
        internal const int FOOD_LIFETIME = 10;
        private readonly IBehaviour behaviour;
        private readonly IFoodGenerator foodGenerator;

        private readonly Dictionary<Vector2Int, int> foods = new();

        private readonly INameGenerator nameGenerator;
        private readonly IStateObserver stateObserver;
        private readonly List<Worm> worms = new();

        internal Simulation(
            INameGenerator nameGenerator_,
            IFoodGenerator foodGenerator_,
            IBehaviour behaviour_,
            IStateObserver stateObserver_
        ) => (nameGenerator, foodGenerator, behaviour, stateObserver) =
            (nameGenerator_, foodGenerator_, behaviour_, stateObserver_);

        public ICollection<Vector2Int> FoodPositions => foods.Keys;
        public IDictionary<Vector2Int, int> Foods => foods;
        public IEnumerable<Worm> Worms => worms;

        public bool IsFood(Vector2Int position) => foods.ContainsKey(position);
        public bool IsWorm(Vector2Int position) => worms.Any(worm => position == worm.Position);

        internal Worm? TrySpawnWorm(Vector2Int position) {
            if (IsWorm(position)) {
                return null;
            }

            var worm = new Worm(nameGenerator.NextName, position);
            worms.Add(worm);
            return worm;
        }

        internal void Run(int steps) {
            if (steps <= 0) {
                throw new ArgumentOutOfRangeException(nameof(steps));
            }

            for (var step = 1; step <= steps; step += 1) {
                Step(step);
                stateObserver.StateChanged(this);
            }
        }

        private void Step(int step) {
            RemoveDecayedFood();
            GenerateFood();
            UpdateWorms(step);
            UpdateFood();
        }

        private void UpdateWorms(int step) {
            for (var i = 0; i < worms.Count;) {
                var worm = worms[i];

                CheckFood(worm);
                var action = behaviour.NextAction(this, worm, step);
                if (TryExecuteAction(worm, action)) {
                    CheckFood(worm);
                }

                worm.DecreaseEnergy();

                if (worm.IsDead) {
                    worms.UnorderedRemove(i);
                } else {
                    i += 1;
                }
            }
        }

        private bool TryExecuteAction(
            Worm worm,
            Action action
        ) {
            switch (action) {
                case Action.Move move:
                    if (IsWorm(worm.Position + move.Direction)) {
                        return false;
                    }

                    worm.Move(move.Direction);
                    return true;
                case Action.Reproduce reproduce:
                    var targetPosition = worm.Position + reproduce.Direction;
                    if (IsWorm(targetPosition)
                        || IsFood(targetPosition)
                        || worm.Energy <= Worm.REPRODUCTION_ENERGY_COST) {
                        return false;
                    }

                    var child = worm.Reproduce(nameGenerator.NextName, reproduce.Direction);
                    worms.Add(child);
                    return true;

                case Action.Nothing:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(action.GetType().Name);
            }
        }

        private void CheckFood(Worm worm) {
            if (!IsFood(worm.Position)) {
                return;
            }

            worm.Eat();
            foods.Remove(worm.Position);
        }

        private void GenerateFood() =>
            foods[foodGenerator.NextFoodPosition(IsFood)] = FOOD_LIFETIME;

        private void UpdateFood() {
            foreach (var position in foods.Keys) {
                foods[position] -= FOOD_DECAY_RATE;
            }
        }

        private void RemoveDecayedFood() {
            var toRemove =
                from item in foods
                where item.Value <= 0
                select item.Key;

            foreach (var position in toRemove) {
                foods.Remove(position);
            }
        }
    }
}