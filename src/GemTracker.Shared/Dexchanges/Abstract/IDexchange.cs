using GemTracker.Shared.Services.Responses.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Dexchanges.Abstract
{
    public interface IDexchange<T, G>
    {
        Task<ListServiceResponse<T>> FetchAllAsync();
        Task<ListLoadedResponse<T, G>> LoadAllAsync();
    }
}