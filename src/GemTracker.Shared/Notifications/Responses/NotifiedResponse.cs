namespace GemTracker.Shared.Notifications.Responses
{
    public class NotifiedResponse
    {
        public string Message { get; set; }
        public bool Success
            => string.IsNullOrWhiteSpace(Message);
    }
}