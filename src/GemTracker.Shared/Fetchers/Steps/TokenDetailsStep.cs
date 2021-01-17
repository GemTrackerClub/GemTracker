using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenDetailsStep : IStep
    {
        private readonly IEthPlorerService _ethPlorerService;
        public TokenDetailsStep(IEthPlorerService ethPlorerService)
        {
            _ethPlorerService = ethPlorerService;
        }
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var detailsData = await _ethPlorerService.FetchTokenInfoAsync(gem.Id);

                var tokenDetails = SharedMessageContent.TokenDetailsContent(gem.Recently, gem.Symbol, detailsData);

                return await Task.FromResult(new StepResult(StepResultType.Success, tokenDetails));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}