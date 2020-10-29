using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class SmartContract
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonIgnore]
        public bool IsVerified
            => !string.IsNullOrWhiteSpace(Message) && string.Equals("OK", Message.ToUpperInvariant());
    }
}