using GemTracker.Shared.Domain.Configs.Abstract;

namespace GemTracker.Shared.Domain.Configs.Services
{
    public class TelegramConfig : ApiConfig
    {
        public AudienceType Audience { get; set; }
        public string ChatId { get; set; }
        public bool IsActive
            => !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(ChatId);
    }
}