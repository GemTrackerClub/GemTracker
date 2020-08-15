using System.ComponentModel;

namespace GemTracker.Shared.Domain.Bot
{
    public enum AuthorCommandType
    {
        [Description("https://cryptodev.com")]
        WEB = 0,

        [Description("https://www.youtube.com/channel/UCDAgUeYcYhnhRaK2MAQGLbw")]
        YOUTUBE = 1,

        [Description("https://twitter.com/tomkowalczyk")]
        TWITTER = 2,

        [Description("https://www.facebook.com/CryptoDevTV")]
        FACEBOOK = 3
    }
}