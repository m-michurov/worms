using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
// ReSharper disable All

namespace Worms.DTO {
#nullable disable
    public sealed class World {
        [JsonPropertyName("worms")]
        public List<Worm> Worms { get; set; }

        [JsonPropertyName("food")]
        public List<Food> Food { get; set; }

        public static World CreateFrom(ISimulationState state) => new() {
            Worms = state.Worms.Select(it => Worm.CreateFrom(it)).ToList(),
            Food = state.Foods.Select(it => DTO.Food.CreateFrom(it.Key, it.Value)).ToList()
        };
    }
}