using GemTracker.Shared.Domain.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Domain
{
    public static class SharedMessageContent
    {
        public static InlineKeyboardButton[] EtherscanButtons(string gemId)
            => new[]
                {
                    InlineKeyboardButton.WithUrl("🔎 EthScan", $"https://etherscan.io/token/{gemId}"),
                    InlineKeyboardButton.WithUrl("📋 Contract", $"https://etherscan.io/address/{gemId}#code"),
                    InlineKeyboardButton.WithUrl("🤑 Hodlers", $"https://etherscan.io/token/{gemId}#balances"),
                };

        public static string RecentlyEmoji(TokenActionType tokenAction)
            => tokenAction == TokenActionType.ADDED || tokenAction == TokenActionType.KYBER_ADDED_TO_ACTIVE
                ? "✅"
                : "❌";

        public static string NetworkEffectContent(TokenActionType tokenAction, string gemSymbol, string gemName)
            => tokenAction == TokenActionType.ADDED || tokenAction == TokenActionType.KYBER_ADDED_TO_ACTIVE
                ?
                    $"📣 *Network effect:*\n" +
                    $"Twitter: [${gemSymbol}](https://twitter.com/search?q=%24{gemSymbol}) | [{gemName}](https://twitter.com/search?q={gemName})\n" +
                    $"Reddit:  [${gemSymbol}](https://www.reddit.com/search/?q=%24{gemSymbol}) | [{gemName}](https://www.reddit.com/search/?q={gemName})\n" +
                    $"4chan:   [${gemSymbol}](https://boards.4channel.org/biz/catalog#s=%24{gemSymbol}) | [{gemName}](https://boards.4channel.org/biz/catalog#s={gemName})\n\n"
                :
                    string.Empty;

        public static string StatisticsContent(TokenActionType tokenAction, string gemId)
            => tokenAction == TokenActionType.ADDED || tokenAction == TokenActionType.KYBER_ADDED_TO_ACTIVE
                ?
                    $"🧮 *Statistics*\n" +
                    $"EthPlorer [{gemId}](https://ethplorer.io/address/{gemId})\n" +
                    $"blockchair [{gemId}](https://blockchair.com/ethereum/erc-20/token/{gemId}?from=gemtracker)\n\n"
                :
                    string.Empty;

        public static string WarningContent(TokenActionType tokenAction, string gemId)
            => tokenAction != TokenActionType.ADDED && tokenAction != TokenActionType.KYBER_ADDED_TO_ACTIVE
                ?
                    $"🛑 *WARNING*\n" +
                    $"Address: [{gemId}](https://etherscan.io/token/{gemId}) \n\n" +
                    $"Make sure to delete the allowance using fe. [revoke.cash](https://revoke.cash)\n\n"
                :
                    string.Empty;
    }
}