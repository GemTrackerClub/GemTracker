using System.ComponentModel;

namespace GemTracker.Shared.Domain.Bot
{
    public enum StartCommandType
    {
        [Description("Commands")]
        COMMANDS = 0,

        [Description("https://t.me/GemTrackerClub")]
        CHANNEL = 1
    }
}