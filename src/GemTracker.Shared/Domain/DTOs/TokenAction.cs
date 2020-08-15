using System.ComponentModel;

namespace GemTracker.Shared.Domain.DTOs
{
    public enum TokenAction
    {
        [Description("Added")]
        ADDED = 0,

        [Description("Deleted")]
        DELETED = 1
    }
}