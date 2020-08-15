using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class TokenList
    {
        [JsonPropertyName("tokens")]
        public List<Token> Tokens { get; set; }
    }
}