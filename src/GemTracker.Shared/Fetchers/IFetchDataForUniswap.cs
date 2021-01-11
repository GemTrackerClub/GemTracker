using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Fetchers.Abstract;
using GemTracker.Shared.Fetchers.Steps;
using GemTracker.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers
{
    public interface IFetchDataForUniswap : IFetchDataForDexchange
    {
        public Task<IEnumerable<FilledUniswapGem>> FetchData(IEnumerable<Gem> gems);
    }

    public class FetchDataForUniswap : IFetchDataForUniswap
    {
        private readonly IUniswapService _uniswapService;
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;

        private ICollection<IStep> _steps;

        public FetchDataForUniswap(
            IUniswapService uniswapService,
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService)
        {
            _uniswapService = uniswapService;
            _etherScanService = etherScanService;
            _ethPlorerService = ethPlorerService;
        }
        public async Task<IEnumerable<FilledUniswapGem>> FetchData(IEnumerable<Gem> gems)
        {
            var formattedResult = new List<FilledUniswapGem>();

            _steps = new HashSet<IStep>
            {
                new TokenRecentlyStep(),
                new TokenNetworkEffectStep(),
                new TokenStatisticsStep(),
                new TokenWarningStep(),

                new TokenChartStep(),

                new TokenDataStep(_uniswapService),
                new TokenDetailsStep(_ethPlorerService)
            };

            foreach (var gem in gems)
            {
                foreach (var step in _steps)
                {
                    var result = await step.ResultAsync(gem);
                    
                    if(result.Success)
                    {

                    }
                }
            }

            return formattedResult;
        }
    }
}