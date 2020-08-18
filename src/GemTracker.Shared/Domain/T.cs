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
    public class T
    {
        private readonly ITelegramService _telegramService;
        public T(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }
        public async Task<Notified> Notify(IEnumerable<Gem> gems)
        {
            var result = new Notified();
            try
            {
                if (gems.AnyAndNotNull())
                {
                    foreach (var gem in gems)
                    {
                        var msg = M.ComposeMessage(gem);
                        var sent = await _telegramService.SendMessageAsync(msg.Item2, msg.Item1);

                        if (!sent.Success)
                        {
                            result.Message += sent.Message;
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