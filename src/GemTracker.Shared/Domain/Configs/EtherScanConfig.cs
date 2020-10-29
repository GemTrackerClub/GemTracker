namespace GemTracker.Shared.Domain.Configs
{
    public class EtherScanConfig
    {
        public string ApiKey { get; set; }
        public bool IsActive
            => !string.IsNullOrWhiteSpace(ApiKey);
    }
}