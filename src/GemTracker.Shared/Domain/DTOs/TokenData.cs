using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class TokenData
    {
        [JsonPropertyName("token")]
        public Token Token { get; set; }
        [JsonPropertyName("priceUSD")]
        public string PriceUSD { get; set; }
        [JsonPropertyName("dailyTxns")]
        public string DailyTxns { get; set; }
        [JsonPropertyName("totalLiquidityUSD")]
        public string TotalLiquidityUSD { get; set; }
        [JsonPropertyName("totalLiquidityToken")]
        public string TotalLiquidityToken { get; set; }
        [JsonPropertyName("totalLiquidityETH")]
        public string TotalLiquidityETH { get; set; }

        private readonly CultureInfo Provider = CultureInfo.InvariantCulture;

        [JsonIgnore]
        public decimal Price
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PriceUSD)
                    ? Math.Round(decimal.Parse(PriceUSD, Provider), 2)
                    : 0;
            }
            set { Price = value; }
        }
        [JsonIgnore]
        public decimal LiquidityUSD
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TotalLiquidityUSD)
                    ? Math.Round(decimal.Parse(TotalLiquidityUSD, Provider), 2)
                    : 0;
            }
            set { LiquidityUSD = value; }
        }
        [JsonIgnore]
        public decimal LiquidityToken
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TotalLiquidityToken)
                    ? Math.Round(decimal.Parse(TotalLiquidityToken, Provider), 2)
                    : 0;
            }
            set { LiquidityToken = value; }
        }
        [JsonIgnore]
        public decimal LiquidityETH
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TotalLiquidityETH)
                    ? Math.Round(decimal.Parse(TotalLiquidityETH, Provider), 2)
                    : 0;
            }
            set { LiquidityETH = value; }
        }
    }
}