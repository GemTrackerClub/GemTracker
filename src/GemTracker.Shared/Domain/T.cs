using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
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
        public async Task<Notified> Notify(IEnumerable<Gem> gems, UniswapApiVersion apiVersion, UniswapEndpoint uniswapEndpoint)
        {
            var result = new Notified();
            try
            {
                if (gems.AnyAndNotNull())
                {
                    foreach (var gem in gems)
                    {
                        //var sent = await _telegramService.SendMessageAsync(M.Compose(gem, apiVersion, uniswapEndpoint));
                        var msg = M.ComposeMessage(gem, apiVersion, uniswapEndpoint);
                        var sent = await _telegramService.SendMessageAsync(msg.Item2, msg.Item1);

                        if (!sent.Success)
                        {
                            result.Message += sent.Message;
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