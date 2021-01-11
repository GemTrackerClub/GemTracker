using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenWarningStep : IStep
    {
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var warningHeader = SharedMessageContent.WarningContent(gem.Recently, gem.Id);

                return await Task.FromResult(new StepResult(StepResultType.Success, warningHeader));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}