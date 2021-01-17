using GemTracker.Shared.Domain.Configs;

namespace GemTracker.Shared.Fetchers.Steps
{
    public interface IStepResult
    {
        string Message { get; }
        bool Success { get; }
    }

    public enum StepResultType
    {
        Success = 0,
        Error = 1
    }

    public class StepResult : IStepResult
    {
        public StepResult() { }
        public StepResult(StepResultType stepResultType, string message, AudienceType audienceType)
        {
            StepResultType = stepResultType;
            Message = message;
            AudienceType = audienceType;
        }
        public string Message { get; private set; }
        public StepResultType StepResultType { get; private set; }
        public AudienceType AudienceType { get; private set; }
        public bool Success => StepResultType == StepResultType.Error;
    }
}