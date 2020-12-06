using GemTracker.Shared.Domain.Configs.Jobs;

namespace GemTracker.Shared.Services.Responses
{
    public class ConfigurationResponse
    {
        public JobFile JobConfig { get; set; }
        public bool Success => !(JobConfig is null);
    }
}