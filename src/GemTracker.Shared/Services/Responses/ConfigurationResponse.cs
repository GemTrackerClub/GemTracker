using GemTracker.Shared.Domain.Configs;

namespace GemTracker.Shared.Services.Responses
{
    public class ConfigurationResponse
    {
        public JobConfigFile JobConfig { get; set; }
        public bool Success => !(JobConfig is null);
    }
}