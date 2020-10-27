using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace GemTracker.Shared.Domain
{
    public class Ntf
    {
        private readonly ITelegramService _telegramService;
        private readonly IUniswapService _uniswapService;
        public Ntf(
            ITelegramService telegramService,
            IUniswapService uniswapService)
        {
            _telegramService = telegramService;
            _uniswapService = uniswapService;
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
                        Thread.Sleep(1000); // to not fall in api limits

                        var tokenData = await _uniswapService.FetchTokenAsync(gem.Id);

                        Thread.Sleep(1000);

                        var pairData = await _uniswapService.FetchPairsAsync(gem.Id);

                        if(tokenData.Success && pairData.Success)
                        {
                            var msgPr = Msg.ForPremiumTelegram(gem, tokenData.TokenData, pairData.Pairs);

                            var sentPr = await _telegramService.SendPremiumMessageAsync(msgPr.Item2, msgPr.Item1);

                            if (!sentPr.Success)
                            {
                                result.Message += $"Telegram Error: {sentPr.Message}";
                            }
                        }

                        var msgTg = Msg.ForFreeTelegram(gem);

                        var sentTg = await _telegramService.SendFreeMessageAsync(msgTg.Item2, msgTg.Item1);

                        if (!sentTg.Success)
                        {
                            result.Message += $"Telegram Error: {sentTg.Message}";
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