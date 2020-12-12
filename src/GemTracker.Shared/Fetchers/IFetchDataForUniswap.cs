using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Fetchers.Abstract;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GemTracker.Shared.Fetchers
{
    public interface IFetchDataForUniswap : IFetchDataForDexchange
    {
    }

    public class FetchDataForUniswap : IFetchDataForUniswap
    {
        private readonly IUniswapService _uniswapService;
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;
        public FetchDataForUniswap(
            IUniswapService uniswapService,
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService)
        {
            _uniswapService = uniswapService;
            _etherScanService = etherScanService;
            _ethPlorerService = ethPlorerService;
        }
        public void FetchData(IEnumerable<Gem> gems)
        {
            throw new NotImplementedException();
        }
    }
}