using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Fetchers.Abstract;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GemTracker.Shared.Fetchers
{
    public interface IFetchDataForKyber : IFetchDataForDexchange
    {
    }

    public class FetchDataForKyber : IFetchDataForKyber
    {
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;
        public FetchDataForKyber(
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService)
        {
            _etherScanService = etherScanService;
            _ethPlorerService = ethPlorerService;
        }
        public void FetchData(IEnumerable<Gem> gems)
        {
            throw new NotImplementedException();
        }
    }
}