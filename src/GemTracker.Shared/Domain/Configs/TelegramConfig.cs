namespace GemTracker.Shared.Domain.Configs
{
    public class TelegramConfig
    {
        public AudienceType Audience { get; set; }
        public string ApiKey { get; set; }
        public string ChatId { get; set; }

        public bool IsActive
            => !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(ChatId);
    }
}