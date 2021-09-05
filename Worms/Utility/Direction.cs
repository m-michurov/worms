namespace Worms.Utility {
    internal readonly struct Direction {
        private readonly Vector2Int offset;

        private Direction(Vector2Int offset) => this.offset = offset;

        public static implicit operator Vector2Int(Direction direction) => direction.offset;

        internal static Direction Up = new(Vector2Int.UnitY);
        internal static Direction Down = new(-Vector2Int.UnitY);
        internal static Direction Left = new(-Vector2Int.UnitX);
        internal static Direction Right = new(Vector2Int.UnitX);

        internal static readonly Direction[] AllDirections = {
            Up, Down, Left, Right
        };
    }
}