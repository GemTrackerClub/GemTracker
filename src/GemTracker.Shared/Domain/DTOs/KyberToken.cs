using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class KyberToken : Token
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}