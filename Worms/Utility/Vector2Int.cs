using System;

namespace Worms.Utility {
    internal readonly struct Vector2Int {
        internal readonly int X;
        internal readonly int Y;

        internal Vector2Int(
            int x,
            int y
        ) => (X, Y) = (x, y);

        public override string ToString() => $"({X}, {Y})";

        public override bool Equals(object? obj) =>
            obj is Vector2Int v && (X, Y) == (v.X, v.Y);

        public override int GetHashCode() => HashCode.Combine(X, Y);

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
        ) => new(v1.X + v2.X, v1.Y + v2.Y);

        public static Vector2Int operator *(
            Vector2Int v1,
            int i
        ) => new(v1.X * i, v1.Y * i);

        public static int Distance(
            Vector2Int v1,
            Vector2Int v2
        ) => Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y);

        public void Deconstruct(
            out int x,
            out int y
        ) => (x, y) = (X, Y);

        public static Vector2Int operator -(Vector2Int v) => new(-v.X, -v.Y);

        internal static readonly Vector2Int Zero = new();
        internal static readonly Vector2Int UnitX = new(1, 0);
        internal static readonly Vector2Int UnitY = new(0, 1);
    }
}