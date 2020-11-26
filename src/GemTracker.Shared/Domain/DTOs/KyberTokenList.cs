using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class KyberTokenList
    {
        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("data")]
        public IEnumerable<KyberToken> Data { get; set; }
    }
}