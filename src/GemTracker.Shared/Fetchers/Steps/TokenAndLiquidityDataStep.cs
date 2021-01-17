using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenAndLiquidityDataStep : IStep
    {
        private readonly IUniswapService _uniswapService;
        public TokenAndLiquidityDataStep(IUniswapService uniswapService)
        {
            _uniswapService = uniswapService;
        }
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var tokenData = await _uniswapService.FetchTokenAsync(gem.Id);

                var tokenInfo = SharedMessageContent.TokenAndLiquidityDataContent(gem.Recently, gem.Symbol, tokenData);

                return new StepResult(StepResultType.Success, tokenInfo, AudienceType.PREMIUM);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage(), AudienceType.PREMIUM));
            }
        }
    }
}