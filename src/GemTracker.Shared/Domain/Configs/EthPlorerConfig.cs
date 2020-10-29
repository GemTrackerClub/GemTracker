namespace GemTracker.Shared.Domain.Configs
{
    public class EthPlorerConfig
    {
        public string ApiKey { get; set; }
        public bool IsActive
            => !string.IsNullOrWhiteSpace(ApiKey);
    }
}