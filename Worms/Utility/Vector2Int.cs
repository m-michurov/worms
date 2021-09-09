using System;

namespace Worms.Utility {
    internal readonly struct Vector2Int {
        private readonly int x;
        private readonly int y;

        internal Vector2Int(
            int x,
            int y
        ) => (this.x, this.y) = (x, y);

        public override string ToString() => $"({x}, {y})";

        public override bool Equals(object? obj) =>
            obj is Vector2Int v && (x, y) == (v.x, v.y);

        public override int GetHashCode() => HashCode.Combine(x, y);

        public static bool operator ==(
            Vector2Int v1,
            Vector2Int v2
        ) => v1.Equals(v2);

        public static bool operator !=(
            Vector2Int v1,
            Vector2Int v2
        ) => !(v1 == v2);

        public static Vector2Int operator +(
            Vector2Int v1,
            Vector2Int v2
        ) => new(v1.x + v2.x, v1.y + v2.y);

        public static Vector2Int operator *(
            Vector2Int v1,
            int i
        ) => new(v1.x * i, v1.y * i);

        public static Vector2Int operator -(Vector2Int v) => new(-v.x, -v.y);

        internal static readonly Vector2Int Zero = new();
        internal static readonly Vector2Int UnitX = new(1, 0);
        internal static readonly Vector2Int UnitY = new(0, 1);
    }
}