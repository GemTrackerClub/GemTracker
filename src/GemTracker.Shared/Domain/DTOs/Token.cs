using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class Token
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }
}