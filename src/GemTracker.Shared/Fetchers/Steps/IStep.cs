using GemTracker.Shared.Domain.DTOs;
using System.Threading.Tasks;

namespace GemTracker.Shared.Fetchers.Steps
{
    public interface IStep
    {
        Task<IStepResult> ResultAsync(Gem gem);
    }
}