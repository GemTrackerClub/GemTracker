using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class TopHolderList
    {
        [JsonPropertyName("holders")]
        public IEnumerable<TopHolder> Holders { get; set; }
    }
}