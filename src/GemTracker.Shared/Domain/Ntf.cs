using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GemTracker.Shared.Domain
{
    public class Ntf
    {
        private readonly ITelegramService _telegramService;
        public Ntf(
            ITelegramService telegramService)
        {
            _telegramService = telegramService;
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
                        var msgTg = Msg.ForFreeTelegram(gem);
                        var sentTg = await _telegramService.SendFreeMessageAsync(msgTg.Item2, msgTg.Item1);

                        if (!sentTg.Success)
                        {
                            result.Message += $"Telegram Error: {sentTg.Message}";
                        }

                        Thread.Sleep(1000); // to not fall in api limits
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