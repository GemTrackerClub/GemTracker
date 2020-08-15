namespace GemTracker.Shared.Domain.Models
{
    public class Notified
    {
        public string Message { get; set; }
        public bool Success
            => string.IsNullOrWhiteSpace(Message);
    }
}