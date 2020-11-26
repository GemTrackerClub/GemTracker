using GemTracker.Shared.Domain.DTOs;
using System.Collections.Generic;

namespace GemTracker.Shared.Domain.Models
{
    public class Loaded<T>
    {
        public IEnumerable<T> OldList { get; set; }
        public List<Gem> OldListDeleted { get; set; }
        public List<Gem> OldListAdded { get; set; }
        public string Message { get; set; }

        public bool Success
            => string.IsNullOrWhiteSpace(Message);
    }
}