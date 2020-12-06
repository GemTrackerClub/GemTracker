using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses.Abstract;
using System.Collections.Generic;

namespace GemTracker.Shared.Services.Responses.Generic
{
    public class ListServiceResponse<T> : ServiceResponse
    {
        public IEnumerable<T> ListResponse { get; set; }
        public bool Success
            => ListResponse.AnyAndNotNull() && string.IsNullOrWhiteSpace(Message);
    }
}