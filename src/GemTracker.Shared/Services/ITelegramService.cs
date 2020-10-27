using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Services
{
    public interface ITelegramService
    {
        Task<SocialResponse> SendFreeMessageAsync(string message, int? messageId = null, IReplyMarkup replyMarkup = null);
        Task<SocialResponse> SendPremiumMessageAsync(string message, int? messageId = null, IReplyMarkup replyMarkup = null);
    }

    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient _telegramFree;
        private readonly ITelegramBotClient _telegramPremium;

        private readonly string _freeChatId;
        private readonly string _premiumChatId;

        private readonly bool _isFreeActive;
        private readonly bool _isPremiumActive;

        public TelegramService(IEnumerable<TelegramConfig> telegramConfigs)
        {
            var freeAudience = telegramConfigs.FirstOrDefault(t => t.Audience == AudienceType.FREE);
            _isFreeActive = freeAudience.IsActive;
            if (_isFreeActive)
            {
                _telegramFree = new TelegramBotClient(freeAudience.ApiKey);
                _freeChatId = freeAudience.ChatId;
            }

            var premiumAudience = telegramConfigs.FirstOrDefault(t => t.Audience == AudienceType.PREMIUM);
            _isPremiumActive = premiumAudience.IsActive;
            if (_isPremiumActive)
            {
                _telegramPremium = new TelegramBotClient(premiumAudience.ApiKey);
                _premiumChatId = premiumAudience.ChatId;
            }
        }

        public async Task<SocialResponse> SendFreeMessageAsync(string message, int? messageId = null, IReplyMarkup replyMarkup = null)
            => await SendMessageAsync(message, _freeChatId, _isFreeActive, messageId, replyMarkup);

        public async Task<SocialResponse> SendPremiumMessageAsync(string message, int? messageId = null, IReplyMarkup replyMarkup = null)
            => await SendMessageAsync(message, _premiumChatId, _isPremiumActive, messageId, replyMarkup);

        private async Task<SocialResponse> SendMessageAsync(string message, string chatId, bool isActive, int? messageId, IReplyMarkup replyMarkup)
        {
            var response = new SocialResponse();
            try
            {
                if(isActive)
                {
                    var s = await _telegramFree.SendTextMessageAsync(
                        chatId,
                        message,
                        parseMode: ParseMode.Markdown,
                        disableWebPagePreview: true,
                        replyToMessageId: messageId.HasValue ? messageId.Value : 0,
                        replyMarkup: replyMarkup);

                    response.MessageId = s.MessageId;
                }

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