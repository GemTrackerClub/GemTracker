using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Services
{
    public interface ITelegramService
    {
        Task<TelegramResponse> SendMessageAsync(string message, IReplyMarkup replyMarkup = null);
    }

    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient _telegramBotClient;

        private readonly string _apikey;
        private readonly string _chatid;

        public TelegramService(
            string apikey,
            string chatid)
        {
            _apikey = apikey;
            _chatid = chatid;

            _telegramBotClient = new TelegramBotClient(apikey);
        }

        public async Task<TelegramResponse> SendMessageAsync(string message, IReplyMarkup replyMarkup = null)
        {
            var response = new TelegramResponse();
            try
            {
                var s = await _telegramBotClient.SendTextMessageAsync(
                    _chatid, 
                    message, 
                    parseMode: ParseMode.Markdown, 
                    disableWebPagePreview: true,
                    replyMarkup: replyMarkup);

                response.Success = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.GetFullMessage();
            }
            return response;
        }
    }
}