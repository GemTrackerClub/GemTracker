namespace GemTracker.Shared.Domain.Configs
{
    public class JobConfigFile
    {
        public string Label { get; set; }
        public bool Notify { get; set; }
        public int ToFetch { get; set; }
    }
}