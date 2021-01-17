using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenRecentlyStep : IStep
    {
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var header =
                    $"{SharedMessageContent.RecentlyEmoji(gem.Recently)}" +
                    $" *{gem.Recently.GetDescription()}* -" +
                    $" {gem.DexType.GetDescription().ToUpperInvariant()}\n\n" +
                    $"💎 Token: *{gem.Name}*\n" +
                    $"🚨 Symbol: *{gem.Symbol}*\n\n";

                return await Task.FromResult(
                    new StepResult(StepResultType.Success, header, AudienceType.FREE));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(
                    new StepResult(StepResultType.Error, ex.GetFullMessage(), AudienceType.FREE));
            }
        }
    }
}