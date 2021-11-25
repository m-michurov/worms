using System.Text.Json.Serialization;

// ReSharper disable All

namespace Worms.DTO {
#nullable disable
    public sealed class Worm {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lifeStrength")]
        public int Energy { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; }

        public static Worm CreateFrom(Worms.Worm worm) => new() {
            Name = worm.Name,
            Energy = worm.Energy,
            Position = DTO.Position.CreateFrom(worm.Position)
        };
    }
}