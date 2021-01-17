using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenContractStep : IStep
    {
        private readonly IEtherScanService _etherScanService;
        public TokenContractStep(IEtherScanService etherScanService)
        {
            _etherScanService = etherScanService;
        }

        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var contractTask = await _etherScanService.IsSmartContractVerifiedAsync(gem.Id);

                var contractDetails = SharedMessageContent.TokenContractContent(gem.Recently, gem.Id, contractTask);

                return new StepResult(StepResultType.Success, contractDetails);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}