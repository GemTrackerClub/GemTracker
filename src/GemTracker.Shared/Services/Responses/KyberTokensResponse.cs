using GemTracker.Shared.Domain.DTOs;

namespace GemTracker.Shared.Services.Responses
{
    public class KyberTokensResponse
    {
        public KyberTokenList List { get; set; }
        public string Message { get; set; }
        public bool Success
            => string.IsNullOrWhiteSpace(Message) && !(List is null);
    }
}