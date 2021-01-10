using GemTracker.Shared.Domain.Enums;
using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class Gem : Token
    {
        [JsonPropertyName("recently")]
        public TokenActionType Recently { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("ispublished")]
        public bool IsPublished { get; set; }
        [JsonIgnore]
        public DexType DexType { get; set; }
        [JsonIgnore]
        public DateTime DateTime
        {
            get 
            {
                CultureInfo provider = CultureInfo.InvariantCulture;

                var dateTime = DateTime.ParseExact(Date, "yyyyMMddHHmmss", provider);

                return dateTime; 
            }
            set { DateTime = value; }
        }
    }
}