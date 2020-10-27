using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class PairList
    {
        [JsonPropertyName("pairs")]
        public List<PairData> Pairs { get; set; }
    }
}