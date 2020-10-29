using GemTracker.Shared.Domain.DTOs;

namespace GemTracker.Shared.Services.Responses
{
    public class EthPlorerTopHoldersResponse
    {
        public TopHolderList HolderList { get; set; }
        public string Message { get; set; }
        public bool Success => !(HolderList is null) && string.IsNullOrWhiteSpace(Message);
    }
}