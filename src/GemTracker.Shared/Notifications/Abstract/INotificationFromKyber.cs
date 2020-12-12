using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Notifications.Responses;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GemTracker.Shared.Notifications.Abstract
{
    public interface INotificationFromKyber : INotification<NotifiedResponse, Gem>
    {
    }
    public class TelegramNotificationFromKyber : INotificationFromKyber
    {
        private readonly ITelegramService _telegramService;
        public TelegramNotificationFromKyber(
            ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }
        public Task<NotifiedResponse> SendAsync(IEnumerable<Gem> gems)
        {
            throw new NotImplementedException();
        }
    }
}