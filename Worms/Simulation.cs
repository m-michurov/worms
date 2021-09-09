using System;
using System.Collections.Generic;
using System.Linq;
using Worms.Behaviour;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

namespace Worms {
    internal sealed class Simulation : ISimulationState {

        private readonly List<Worm> worms = new();

        private readonly INameGenerator nameGenerator;
        private readonly IBehaviour behaviour;
        private readonly IStateObserver stateObserver;

        internal Simulation(
            INameGenerator nameGenerator_,
            IBehaviour behaviour_,
            IStateObserver stateObserver_
        ) => (nameGenerator, behaviour, stateObserver) =
            (nameGenerator_, behaviour_, stateObserver_);
        
        public IEnumerable<string> Worms => from worm in worms select worm.ToString();
        
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

            for (var i = 0; i < steps; i += 1) {
                Step();
                stateObserver.StateChanged(this);
            }
        }

        private void Step() => UpdateWorms();

        private void UpdateWorms() {
            for (var i = 0; i < worms.Count; i += 1) {
                var worm = worms[i];

                var action = behaviour.NextAction(this, worm);
                _ = TryExecuteAction(worm, action);
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

                case Action.Nothing:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(action.GetType().Name);
            }
        }
    }
}