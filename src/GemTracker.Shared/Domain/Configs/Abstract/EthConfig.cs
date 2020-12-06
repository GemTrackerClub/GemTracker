namespace GemTracker.Shared.Domain.Configs.Abstract
{
    public abstract class EthConfig : ApiConfig
    {
        public bool IsActive
            => !string.IsNullOrWhiteSpace(ApiKey);
    }
}