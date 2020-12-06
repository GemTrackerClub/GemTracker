using GemTracker.Shared.Domain.Configs.Abstract;

namespace GemTracker.Shared.Domain.Configs.Services
{
    public class TwitterConfig : ApiConfig
    {
        public string ApiSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
    }
}