using System.Text.Json.Serialization;
using Worms.Utility;

// ReSharper disable All

namespace Worms.DTO {
#nullable disable
    public sealed class Food {
        [JsonPropertyName("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; }

        public static Food CreateFrom(
            Vector2Int position,
            int expiresIn
        ) => new() {
            ExpiresIn = expiresIn,
            Position = Position.CreateFrom(position)
        };
    }
}