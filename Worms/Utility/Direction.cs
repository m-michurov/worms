using System;

namespace Worms.Utility {
    public readonly struct Direction {
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

        public override string ToString() {
            if (Up == offset) {
                return nameof(Up);
            }
            if (Down == offset) {
                return nameof(Down);
            }
            if (Left == offset) {
                return nameof(Left);
            }
            return Right == offset ? nameof(Right) : offset.ToString();
        }

        internal static Direction FromString(string direction) => direction switch {
            nameof(Up) => Up,
            nameof(Down) => Down,
            nameof(Left) => Left,
            nameof(Right) => Right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}