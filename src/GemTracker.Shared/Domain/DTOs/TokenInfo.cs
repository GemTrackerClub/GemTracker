using System;
using System.Text.Json.Serialization;

namespace GemTracker.Shared.Domain.DTOs
{
    public class TokenInfo
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("decimals")]
        public string Decimals { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("totalSupply")]
        public string TotalSupply { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("transfersCount")]
        public long TransfersCount { get; set; }

        [JsonPropertyName("lastUpdated")]
        public long LastUpdated { get; set; }

        [JsonPropertyName("issuancesCount")]
        public long IssuancesCount { get; set; }

        [JsonPropertyName("holdersCount")]
        public long HoldersCount { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("website")]
        public Uri Website { get; set; }

        [JsonPropertyName("facebook")]
        public string Facebook { get; set; }

        [JsonPropertyName("telegram")]
        public Uri Telegram { get; set; }

        [JsonPropertyName("twitter")]
        public string Twitter { get; set; }

        [JsonPropertyName("reddit")]
        public string Reddit { get; set; }

        [JsonPropertyName("coingecko")]
        public string Coingecko { get; set; }

        [JsonPropertyName("ethTransfersCount")]
        public long EthTransfersCount { get; set; }

        [JsonPropertyName("countOps")]
        public long CountOps { get; set; }
    }
}