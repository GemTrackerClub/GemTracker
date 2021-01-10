using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
            throw new NotImplementedException();
        }
    }
}