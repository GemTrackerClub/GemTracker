using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenPairsStep : IStep
    {
        private readonly IUniswapService _uniswapService;
        public TokenPairsStep(IUniswapService uniswapService)
        {
            _uniswapService = uniswapService;
        }
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var pairTask = await _uniswapService.FetchPairsAsync(gem.Id);

                var pairsDetails = SharedMessageContent.TokenPairsContent(gem.Recently, pairTask);

                return new StepResult(StepResultType.Success, pairsDetails, AudienceType.PREMIUM);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(
                    new StepResult(StepResultType.Error, ex.GetFullMessage(), AudienceType.PREMIUM));
            }
        }
    }
}