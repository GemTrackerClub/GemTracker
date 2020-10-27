using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class TokenDataList
    {
        [JsonPropertyName("tokenDayDatas")]
        public List<TokenData> TokenDayDatas { get; set; }
    }
}