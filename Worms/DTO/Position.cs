using System.Text.Json.Serialization;
using Worms.Utility;

// ReSharper disable All

namespace Worms.DTO {
    public sealed class Position {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        public static Position CreateFrom(Vector2Int vector) => new() {
            X = vector.X,
            Y = vector.Y
        };
    }
}