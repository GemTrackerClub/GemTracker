using GemTracker.Shared.Services.Responses.Abstract;

namespace GemTracker.Shared.Services.Responses.Generic
{
    public class SingleServiceResponse<T> : ServiceResponse
    {
        public T ObjectResponse { get; set; }
        public bool Success
            => !(ObjectResponse is null) && string.IsNullOrWhiteSpace(Message);
    }
}