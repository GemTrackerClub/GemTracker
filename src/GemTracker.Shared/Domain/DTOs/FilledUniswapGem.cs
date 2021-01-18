namespace GemTracker.Shared.Domain.DTOs
{
    public class FilledUniswapGem : Gem
    {
        public string FreeGemMessage { get; set; }
        public string PremiumGemMessage { get; set; }
        public string FilteredGemMessage { get; set; }
    }
}