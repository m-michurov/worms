using System.Text.Json.Serialization;
// ReSharper disable All

namespace Worms.DTO {
#nullable disable
    public sealed class Response {
        [JsonPropertyName("action")]
        public Action Action { get; set; }
    }
}