using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class PairData
    {
        [JsonPropertyName("createdAtTimestamp")]
        public string CreatedAtTimestamp { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("reserveUSD")]
        public string ReserveUSD { get; set; }
        [JsonPropertyName("token0")]
        public Token Token0 { get; set; }
        [JsonPropertyName("token1")]
        public Token Token1 { get; set; }

        private readonly CultureInfo Provider = CultureInfo.InvariantCulture;

        public DateTime CreatedAt
        {
            get
            {
                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(CreatedAtTimestamp, Provider));
                return dateTimeOffset.DateTime;
            }
            set { CreatedAt = value; }
        }

        public decimal TotalLiquidityUSD
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ReserveUSD)
                    ? Math.Round(decimal.Parse(ReserveUSD, Provider), 2)
                    : 0;
            }
            set { TotalLiquidityUSD = value; }
        }
    }
}