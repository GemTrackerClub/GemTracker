using System.Collections.Generic;

namespace GemTracker.Shared.Services.Responses.Generic
{
    public class ListLoadedResponse<T, G>
    {
        public IEnumerable<T> OldList { get; set; }
        public List<G> OldListDeleted { get; set; }
        public List<G> OldListAdded { get; set; }
        public string Message { get; set; }

        public bool Success
            => string.IsNullOrWhiteSpace(Message);
    }
}