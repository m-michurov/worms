using System.Text.Json.Serialization;

// ReSharper disable All

namespace Worms.DTO {
#nullable disable
    public sealed class Action {
        [JsonPropertyName("direction")]
        public string Direction { get; set; }

        [JsonPropertyName("split")]
        public bool Split { get; set; }

        internal Worms.Action ToWormAction() {
            var diresction = Utility.Direction.FromString(Direction);
            return Split ? new Worms.Action.Reproduce(diresction) : new Worms.Action.Move(diresction);
        }
    }
}