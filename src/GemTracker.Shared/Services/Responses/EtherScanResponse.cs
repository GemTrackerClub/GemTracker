using GemTracker.Shared.Domain.DTOs;

namespace GemTracker.Shared.Services.Responses
{
    public class EtherScanResponse
    {
        public SmartContract Contract { get; set; }
        public bool Success => !(Contract is null) && string.IsNullOrWhiteSpace(Message);
        public string Message { get; set; }
    }
}