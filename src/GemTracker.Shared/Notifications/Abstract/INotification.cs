using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Notifications.Abstract
{
    public interface INotification<N, T>
    {
        Task<N> SendAsync(IEnumerable<T> gems);
    }
}