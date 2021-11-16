using System;
using System.Collections.Generic;
using Worms.Database;
using Worms.Utility;

namespace Worms.Food {
    internal sealed class LazyDbWorldBehaviorLoader : IFoodGenerator, IDisposable {
        private readonly string behaviorName;
        private readonly IWorldBehaviorsRepository repository;

        private IEnumerator<Vector2Int>? positionsEnumerator;

        public LazyDbWorldBehaviorLoader(
            IWorldBehaviorsRepository repository_,
            string behaviorName_
        ) => (repository, behaviorName) = (repository_, behaviorName_);

        public void Dispose() => positionsEnumerator?.Dispose();

        public Vector2Int NextFoodPosition(Predicate<Vector2Int> isOccupied) {
            if (positionsEnumerator is null) {
                LoadBehavior();
            }

            var elementsLeft = positionsEnumerator!.MoveNext();
            if (false == elementsLeft) {
                throw new ArgumentException("no more food positions");
            }

            return positionsEnumerator.Current;
        }

        private void LoadBehavior() {
            var worldBehavior = repository.GetByName(behaviorName);
            if (worldBehavior is null) {
                throw new ArgumentException($"world behaviour with name \"{behaviorName} does not exist");
            }

            var foodPositions = worldBehavior.ToFoodPositions();
            positionsEnumerator?.Dispose();
            positionsEnumerator = foodPositions.GetEnumerator();
        }
    }
}