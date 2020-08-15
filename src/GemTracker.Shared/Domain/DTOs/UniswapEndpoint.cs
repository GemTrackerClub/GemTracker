using System.ComponentModel;

namespace GemTracker.Shared.Domain.DTOs
{
    public enum UniswapEndpoint
    {
        [Description("1k")]
        REST = 0,

        [Description("all")]
        GRAPH = 1
    }
}