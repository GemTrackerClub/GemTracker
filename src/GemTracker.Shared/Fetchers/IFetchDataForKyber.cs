using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Fetchers.Abstract;
using GemTracker.Shared.Fetchers.Steps;
using GemTracker.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers
{
    public interface IFetchDataForKyber : IFetchDataForDexchange
    {
        public Task<IEnumerable<FilledKyberGem>> FetchData(IEnumerable<Gem> gems);
    }

    public class FetchDataForKyber : IFetchDataForKyber
    {
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;

        private ICollection<IStep> _steps;

        public FetchDataForKyber(
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService)
        {
            _etherScanService = etherScanService;
            _ethPlorerService = ethPlorerService;
        }
        public async Task<IEnumerable<FilledKyberGem>> FetchData(IEnumerable<Gem> gems)
        {
            var formattedResult = new List<FilledKyberGem>();

            _steps = new HashSet<IStep>
            {
                new TokenRecentlyStep(),
                new TokenNetworkEffectStep(),
                new TokenStatisticsStep(),
                new TokenWarningStep(),

                new TokenDetailsStep(_ethPlorerService),
                new TokenContractStep(_etherScanService)
            };

            foreach (var gem in gems)
            {
                foreach (var step in _steps)
                {
                    var result = await step.ResultAsync(gem);

                    if (result.Success)
                    {

                    }
                }
            }

            return formattedResult;
        }
    }
}