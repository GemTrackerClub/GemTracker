using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public class TokenChartStep : IStep
    {
        public async Task<IStepResult> ResultAsync(Gem gem)
        {
            try
            {
                var chartHeader = SharedMessageContent.ChartsContent(gem.Recently, gem.Symbol);

                return await Task.FromResult(new StepResult(StepResultType.Success, chartHeader));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new StepResult(StepResultType.Error, ex.GetFullMessage()));
            }
        }
    }
}