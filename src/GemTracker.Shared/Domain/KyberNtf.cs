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
    public class KyberNtf
    {
        private readonly ITelegramService _telegramService;
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;

        public KyberNtf(
            ITelegramService telegramService,
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService
            )
        {
            _telegramService = telegramService;
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
                        Task<EthPlorerTokenInfoResponse> tokenInfoTask = null;
                        Task<EtherScanResponse> contractTask = null;
                        Task<EthPlorerTopHoldersResponse> holdersTask = null;

                        await TaskExt.Sequence(
                            () => { return tokenInfoTask = _ethPlorerService.FetchTokenInfoAsync(gem.Id); },
                            () => { return contractTask = _etherScanService.IsSmartContractVerifiedAsync(gem.Id); },
                            () => { return Task.Delay(1000); },
                            () => { return holdersTask = _ethPlorerService.FetchTopHoldersAsync(gem.Id, 5); }
                        );

                        var msgPr = KyberMsg.ForPremiumTelegram(
                            gem,
                            tokenInfoTask.Result,
                            contractTask.Result,
                            holdersTask.Result);

                        var sentPr = await _telegramService.SendPremiumMessageAsync(msgPr.Item2, msgPr.Item1);

                        if (!sentPr.Success)
                        {
                            result.Message += $"Telegram Premium Error: {sentPr.Message}";
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