using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Domain
{
    public class M
    {
        public static string Compose(Gem gem, UniswapApiVersion uniswapApiVersion, UniswapEndpoint uniswapEndpoint)
        {
            var banner = $"\n\n*{gem.Recently.GetDescription()}* - Uniswap ({uniswapApiVersion.GetDescription()}): *{gem.Name}* _({gem.Symbol})_\n\n";

            var uniswapTokenInfo = $"[Info](https://uniswap.info/token/{gem.Id})";
            var uniswapTokenBuy = $"[Buy](https://app.uniswap.org/#/swap?outputCurrency={gem.Id})";
            var uniswapTokenSell = $"[Sell](https://app.uniswap.org/#/swap?inputCurrency={gem.Id})";
            
            var etherscanTokenInfo = $"[Info](https://etherscan.io/token/{gem.Id})";

            var authorBanner = $"[CryptoDev](https://cryptodev.tv)";
            var joinTelegram = $"[Telegram](https://t.me/joinchat/JmoiyRyhQp5o7Ts1ZezFQA)";
            var giveAuthorSubOnYT = $"[YouTube](https://www.youtube.com/channel/UCDAgUeYcYhnhRaK2MAQGLbw?sub_confirmation=1)";
            var authorTwitter = $"[Twitter](https://twitter.com/tomkowalczyk)";

            return $"{banner}" +
                $"Uniswap *(change in {uniswapEndpoint.GetDescription()} tokens)*:\n\n" +
                $"{uniswapTokenInfo} | {uniswapTokenBuy} | {uniswapTokenSell}\n\n" +
                $"Etherscan:\n\n" +
                $"{etherscanTokenInfo}" +
                $"\n\n" +
                $"*Visit* [GemTracker.club](https://gemtracker.club)" +
                $"\n" +
                $"*say thx to* {authorBanner} | {joinTelegram} | {giveAuthorSubOnYT} | {authorTwitter}";
        }

        public static Tuple<IReplyMarkup, string> ComposeMessage(Gem gem, UniswapApiVersion uniswapApiVersion, UniswapEndpoint uniswapEndpoint)
        {
            var authorBanner = $"[CryptoDev](https://cryptodev.tv)";
            var joinTelegram = $"[Telegram](https://t.me/joinchat/JmoiyRyhQp5o7Ts1ZezFQA)";
            var giveAuthorSubOnYT = $"[YouTube](https://www.youtube.com/channel/UCDAgUeYcYhnhRaK2MAQGLbw?sub_confirmation=1)";
            var authorTwitter = $"[Twitter](https://twitter.com/tomkowalczyk)";

            var emoji = gem.Recently == TokenAction.ADDED
                ? "✅"
                : "❌";

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
                $"blockchair [{gem.Id}](https://blockchair.com/ethereum/erc-20/token/{gem.Id}?from=gemtracker)\n" +
                $"@GemTrackerClubBot /fr lala\n" +
                $"🙏 *thx to* {authorBanner} | {joinTelegram} | {giveAuthorSubOnYT} | {authorTwitter}";

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithUrl("📊 Info", $"https://uniswap.info/token/{gem.Id}"),
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
                    InlineKeyboardButton.WithUrl("How to use? 🚀 GemTracker.club", $"https://gemtracker.club")
                }
            }); ;

            return new Tuple<IReplyMarkup, string>(buttons, banner);
        }
    }
}