using GemTracker.Shared.Domain.DTOs;

namespace GemTracker.Shared.Services.Responses
{
    public class UniswapTokenDataResponse
    {
        public TokenData TokenData { get; set; }
        public string Message { get; set; }
        public bool Success
            => string.IsNullOrWhiteSpace(Message) && !(TokenData is null);
    }
}