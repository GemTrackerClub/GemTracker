using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenStatisticsStep : IStep
    {
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var statisticsHeader = SharedMessageContent.StatisticsContent(gem.Recently, gem.Id);

                return await Task.FromResult(
                    new StepResult(StepResultType.Success, statisticsHeader, AudienceType.FREE));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(
                    new StepResult(StepResultType.Error, ex.GetFullMessage(), AudienceType.FREE));
            }
        }
    }
}