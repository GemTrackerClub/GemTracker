using GemTracker.Shared.Domain.DTOs;
using System.Collections.Generic;

namespace GemTracker.Shared.Fetchers.Abstract
{
    public interface IFetchDataForDexchange
    {
        public void FetchData(IEnumerable<Gem> gems);
    }
}