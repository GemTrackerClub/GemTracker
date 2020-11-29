using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using GemTracker.Shared.Services.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Domain
{
    public class UniNtf
    {
        private readonly ITelegramService _telegramService;
        private readonly IUniswapService _uniswapService;
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;
        public UniNtf(
            ITelegramService telegramService,
            IUniswapService uniswapService,
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService)
        {
            _telegramService = telegramService;
            _uniswapService = uniswapService;
            _etherScanService = etherScanService;
            _ethPlorerService = ethPlorerService;
        }
        public async Task<Notified> SendAsync(IEnumerable<Gem> gems)
        {
            var result = new Notified();
            try
            {
                if (gems.AnyAndNotNull())
                {
                    foreach (var gem in gems)
                    {
                        Task<UniswapTokenDataResponse> uniTokenTask = null;
                        Task<EthPlorerTokenInfoResponse> tokenInfoTask = null;
                        Task<EtherScanResponse> contractTask = null;
                        Task<UniswapPairDataResponse> pairTask = null;
                        Task<EthPlorerTopHoldersResponse> holdersTask = null;

                        await TaskExt.Sequence(
                            () => { return uniTokenTask = _uniswapService.FetchTokenAsync(gem.Id); },
                            () => { return tokenInfoTask = _ethPlorerService.FetchTokenInfoAsync(gem.Id); },
                            () => { return contractTask = _etherScanService.IsSmartContractVerifiedAsync(gem.Id); },
                            () => { return Task.Delay(1000); },
                            () => { return pairTask = _uniswapService.FetchPairsAsync(gem.Id); },
                            () => { return holdersTask = _ethPlorerService.FetchTopHoldersAsync(gem.Id, 5); }
                        );

                        var msgPr = UniMsg.ForPremiumTelegram(
                            gem,
                            uniTokenTask.Result,
                            tokenInfoTask.Result,
                            contractTask.Result,
                            pairTask.Result,
                            holdersTask.Result);

                        var sentPr = await _telegramService.SendPremiumMessageAsync(msgPr.Item2, msgPr.Item1);

                        if (!sentPr.Success)
                        {
                            result.Message += $"Telegram Premium Error: {sentPr.Message}";
                        }

                        var msgTg = UniMsg.ForFreeTelegram(gem);

                        var sentTg = await _telegramService.SendFreeMessageAsync(msgTg.Item2, msgTg.Item1);

                        if (!sentTg.Success)
                        {
                            result.Message += $"Telegram Free Error: {sentTg.Message}";
                        }
                    }
                }
                else
                    result.Message = "Nothing to send";
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
    }
}