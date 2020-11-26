using System.ComponentModel;

namespace GemTracker.Shared.Domain.Enums
{
    public enum DexType
    {
        [Description("uniswap")]
        UNISWAP = 0,

        [Description("kyber")]
        KYBER = 1
    }
}