namespace GemTracker.Shared.Domain.Configs.Jobs
{
    public class Job
    {
        public string Name { get; set; }
        public int IntervalInMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}