using System.Collections.Generic;

namespace Worms.Database {
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class WorldBehavior {
        internal WorldBehavior() => (Name, FoodPositions) = ("", new List<FoodPosition>());

        public int Id { get; set; }

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public string Name { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        internal ICollection<FoodPosition> FoodPositions { get; set; }

        public override string ToString() =>
            $"{nameof(Id)}={Id} {nameof(Name)}={Name}";
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class FoodPosition {
        public int WorldBehaviorId { get; set; }

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public int Step { get; set; }

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public int X { get; set; }

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public int Y { get; set; }

        public override string ToString() =>
            $"{nameof(WorldBehaviorId)}={WorldBehaviorId} {nameof(Step)}={Step} {nameof(X)}={X} {nameof(Y)}={Y}";
    }
}