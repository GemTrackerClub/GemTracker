using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenHoldersStep : IStep
    {
        private readonly IEthPlorerService _ethPlorerService;
        public TokenHoldersStep(IEthPlorerService ethPlorerService)
        {
            _ethPlorerService = ethPlorerService;
        }
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var topHodlersNumber = 5;

                var holdersTask = await _ethPlorerService.FetchTopHoldersAsync(gem.Id, topHodlersNumber);

                var detailsHolders = SharedMessageContent.TokenHoldersContent(gem.Recently, gem.Id, topHodlersNumber, holdersTask);

                return new StepResult(StepResultType.Success, detailsHolders);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}