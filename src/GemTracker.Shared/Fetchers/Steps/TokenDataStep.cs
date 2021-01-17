using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenDataStep : IStep
    {
        private readonly IUniswapService _uniswapService;
        public TokenDataStep(IUniswapService uniswapService)
        {
            _uniswapService = uniswapService;
        }
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var tokenData = await _uniswapService.FetchTokenAsync(gem.Id);

                var tokenInfo = SharedMessageContent.TokenDataContent(gem.Recently, gem.Symbol, tokenData);

                return await Task.FromResult(new StepResult(StepResultType.Success, tokenInfo));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}