using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class TopHolder
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("share")]
        public decimal Share { get; set; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
    }
}