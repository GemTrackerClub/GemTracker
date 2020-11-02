using GemTracker.Shared.Domain.DTOs;

namespace GemTracker.Shared.Services.Responses
{
    public class EthPlorerTokenInfoResponse
    {
        public TokenInfo TokenInfo { get; set; }
        public string Message { get; set; }
        public bool Success => !(TokenInfo is null) && string.IsNullOrWhiteSpace(Message);
    }
}