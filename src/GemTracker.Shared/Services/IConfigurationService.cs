using GemTracker.Shared.Domain.Configs.Jobs;
using GemTracker.Shared.Domain.Statics;
using GemTracker.Shared.Services.Responses;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IConfigurationService
    {
        Task<ConfigurationResponse> GetJobConfigAsync(string fileName);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IFileService _fileService;
        public ConfigurationService(IFileService fileService)
        {
            _fileService = fileService;
        }
        public async Task<ConfigurationResponse> GetJobConfigAsync(string fileName)
        {
            return new ConfigurationResponse
            {
                JobConfig = await _fileService.GetAsync<JobFile>(PathTo.Job(fileName))
            };
        }
    }
}