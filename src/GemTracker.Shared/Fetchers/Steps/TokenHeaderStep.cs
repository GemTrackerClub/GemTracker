using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenHeaderStep : IStep
    {
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var header =
                    $"{SharedMessageContent.RecentlyEmoji(gem.Recently)}" +
                    $" *{gem.Recently.GetDescription()}* -" +
                    $" {gem.DexType.GetDescription().ToUpperInvariant()}\n\n";

                return await Task.FromResult(new StepResult(StepResultType.Success, header));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}