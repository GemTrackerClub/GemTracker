using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenNetworkEffectStep : IStep
    {
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var networkHeader = SharedMessageContent.NetworkEffectContent(gem.Recently, gem.Symbol, gem.Name);

                return await Task.FromResult(
                    new StepResult(StepResultType.Success, networkHeader, AudienceType.FREE));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(
                    new StepResult(StepResultType.Error, ex.GetFullMessage(), AudienceType.FREE));
            }
        }
    }
}