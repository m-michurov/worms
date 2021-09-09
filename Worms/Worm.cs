using Worms.Utility;

namespace Worms {
    internal sealed class Worm {
        private readonly string name;
        internal Vector2Int Position { get; private set; }

        internal Worm(
            string name,
            Vector2Int position
        ) => (this.name, Position) = (name, position);

        internal void Move(Direction direction) => Position += direction;

        public override string ToString() => $"{name} {Position}";

        public override bool Equals(object? obj) => 
            obj is Worm other && other.name == name;

        public override int GetHashCode() => name.GetHashCode();
    }
}