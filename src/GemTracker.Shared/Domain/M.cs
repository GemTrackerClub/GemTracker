using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Domain
{
    public class M
    {
        public static string MessageForTwitter(Gem gem)
        {
            var emoji = gem.Recently == TokenAction.ADDED
                ? "✅"
                : "❌";

            var result =
                $"{emoji} {gem.Recently.GetDescription()} - Uniswap\n\n" +
                $"💎 Token: {gem.Name}\n" +
                $"🚨 Symbol: ${gem.Symbol}\n" +
                $"🦄 Uniswap: https://uniswap.info/token/{gem.Id} \n" +
                $"🔎 EthScan: https://etherscan.io/token/{gem.Id} \n" +
                $"Join: https://t.me/GemTrackerClub \n" +
                $"( $BTC $ETH $ALTS $UNI #uniswap #cryptocurrency #gem)\n";

            return result;
        }
        public static Tuple<IReplyMarkup, string> MessageForTelegram(Gem gem)
        {
            var emoji = gem.Recently == TokenAction.ADDED
                ? "✅"
                : "❌";

            var uniswapApiVersion = UniswapApiVersion.V2;
            var uniswapEndpoint = UniswapEndpoint.GRAPH;

            var banner = $"\n\n {emoji} *{gem.Recently.GetDescription()}* - Uniswap ({uniswapApiVersion.GetDescription()})\n\n" +
                $"🦄 Uniswap *(change in {uniswapEndpoint.GetDescription()} tokens)*\n\n" +
                $"💎 Token: *{gem.Name}*\n" +
                $"🚨 Symbol: *{gem.Symbol}*\n\n" +
                $"📣 *Network effect:*\n" +
                $"Twitter mentions [${gem.Symbol}](https://twitter.com/search?q=%24{gem.Symbol})\n" +
                $"Reddit questions [{gem.Name} {gem.Symbol}](https://www.reddit.com/search/?q={gem.Symbol})\n" +
                $"4chan hype [{gem.Symbol}](https://boards.4channel.org/search#/{gem.Symbol}/biz)\n\n" +
                $"🧮 *Statistics*\n" +
                $"EthPlorer [{gem.Id}](https://ethplorer.io/address/{gem.Id})\n" +
                $"blockchair [{gem.Id}](https://blockchair.com/ethereum/erc-20/token/{gem.Id}?from=gemtracker)\n\n" +
                $"📊 *Charts*\n" +
                $"ChartEx [${gem.Symbol}](https://chartex.pro/?symbol=UNISWAP:{gem.Symbol}) | [${gem.Symbol}/$WETH](https://chartex.pro/?symbol=UNISWAP:{gem.Symbol}/WETH)\n\n" +
                $"👨‍👦‍👦 Our community:\n" +
                $"Chat - @GemTrackerCommunity\n" +
                $"Info - @GemTrackerAnnouncements\n";

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithUrl("📋 Info", $"https://uniswap.info/token/{gem.Id}"),
                    InlineKeyboardButton.WithUrl("📉 Buy", $"https://app.uniswap.org/#/swap?outputCurrency={gem.Id}"),
                    InlineKeyboardButton.WithUrl("📈 Sell", $"https://app.uniswap.org/#/swap?inputCurrency={gem.Id}"),
                },
                new []
                {
                    InlineKeyboardButton.WithUrl("🔎 EthScan", $"https://etherscan.io/token/{gem.Id}"),
                    InlineKeyboardButton.WithUrl("📋 Contract", $"https://etherscan.io/address/{gem.Id}"),
                    InlineKeyboardButton.WithUrl("🤑 Hodlers", $"https://etherscan.io/token/{gem.Id}#balances"),
                },
                new []
                {
                    InlineKeyboardButton.WithUrl("⚙️ How to use?", $"https://gemtracker.club/#howtouse"),
                    InlineKeyboardButton.WithUrl("💰 Premium access?", $"https://gemtracker.club/#premium")
                },
                new []
                {
                    InlineKeyboardButton.WithUrl("📧 Author", $"https://twitter.com/tomkowalczyk")
                }
            }); ;

            return new Tuple<IReplyMarkup, string>(buttons, banner);
        }
    }
}