using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System.Collections.Generic;

namespace GemTracker.Shared.Services.Responses
{
    public class UniswapResponse
    {
        public IEnumerable<Token> Tokens { get; set; }
        public string Message { get; set; }
        public bool Success
            => string.IsNullOrWhiteSpace(Message) && Tokens.AnyAndNotNull();
    }
}