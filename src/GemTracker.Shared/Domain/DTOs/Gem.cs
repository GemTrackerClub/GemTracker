using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class Gem : Token
    {
        [JsonPropertyName("recently")]
        public TokenAction Recently { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("ispublished")]
        public bool IsPublished { get; set; }
    }
}