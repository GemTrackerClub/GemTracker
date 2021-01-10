using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
            throw new NotImplementedException();
        }
    }
}