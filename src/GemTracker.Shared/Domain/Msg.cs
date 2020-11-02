using GemTracker.Shared.Converters;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Domain
{
    public class Msg
    {
        public static string ForTwitterSummary(IEnumerable<Gem> gems, TokenAction tokenAction, int interval)
        {
            var emoji = tokenAction == TokenAction.ADDED
                ? "✅"
                : "❌";

            var result =
                $"🦄 Uniswap (for last {interval / 60} h) \n\n" +
                $" {emoji} {tokenAction.GetDescription()} \n\n" +
                $"💎 {gems.Count()} Tokens\n" +
                $"🚨 Some of them: {string.Join(" ", gems.Take(5).Select(g => $"${g.Symbol}"))}\n\n" +
                $"Join for free: https://t.me/GemTrackerClub \n" +
                $"💰 Ask for premium: https://gemtracker.club/#premium \n" +
                $"( $BTC $ETH $ALTS $UNI #uniswap #cryptocurrency #gem #gemtrackerclub )";

            return result;
        }
        public static string ForTwitter(Gem gem)
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
                $"Join for free: https://t.me/GemTrackerClub \n" +
                $"💰 Ask for premium: https://gemtracker.club/#premium \n" +
                $"( $BTC $ETH $ALTS $UNI #uniswap #cryptocurrency #gem)\n";

            return result;
        }
        private static string RecentlyEmoji(Gem gem)
            => gem.Recently == TokenAction.ADDED
                ? "✅"
                : "❌";
        private static InlineKeyboardButton[] UniswapButtons(Gem gem)
        {
            var buyOrNot
                = gem.Recently == TokenAction.ADDED
                ? InlineKeyboardButton.WithUrl("📉 Buy", $"https://app.uniswap.org/#/swap?outputCurrency={gem.Id}")
                : InlineKeyboardButton.WithUrl("🛑 DON'T Buy it", $"https://uniswap.info/");

            return new[]
                {
                    InlineKeyboardButton.WithUrl("📋 Info", $"https://uniswap.info/token/{gem.Id}"),
                    buyOrNot,
                    InlineKeyboardButton.WithUrl("📈 Sell", $"https://app.uniswap.org/#/swap?inputCurrency={gem.Id}"),
                };
        }
        private static InlineKeyboardButton[] EtherscanButtons(Gem gem)
            => new[]
                {
                    InlineKeyboardButton.WithUrl("🔎 EthScan", $"https://etherscan.io/token/{gem.Id}"),
                    InlineKeyboardButton.WithUrl("📋 Contract", $"https://etherscan.io/address/{gem.Id}"),
                    InlineKeyboardButton.WithUrl("🤑 Hodlers", $"https://etherscan.io/token/{gem.Id}#balances"),
                };
        private static string Content(Gem gem)
        {
            var networkEffectVisible
                = gem.Recently == TokenAction.ADDED
                ?
                $"📣 *Network effect:*\n" +
                $"Twitter: [${gem.Symbol}](https://twitter.com/search?q=%24{gem.Symbol}) | [{gem.Name}](https://twitter.com/search?q={gem.Name})\n" +
                $"Reddit:  [${gem.Symbol}](https://www.reddit.com/search/?q=%24{gem.Symbol}) | [{gem.Name}](https://www.reddit.com/search/?q={gem.Name})\n" +
                $"4chan:   [${gem.Symbol}](https://boards.4channel.org/biz/catalog#s=%24{gem.Symbol}) | [{gem.Name}](https://boards.4channel.org/biz/catalog#s={gem.Name})\n\n"
                :
                string.Empty;

            var statisticsVisible
                = gem.Recently == TokenAction.ADDED
                ?
                $"🧮 *Statistics*\n" +
                $"EthPlorer [{gem.Id}](https://ethplorer.io/address/{gem.Id})\n" +
                $"blockchair [{gem.Id}](https://blockchair.com/ethereum/erc-20/token/{gem.Id}?from=gemtracker)\n\n"
                :
                string.Empty;

            var chartsVisible
                = gem.Recently == TokenAction.ADDED
                ?
                $"📊 *Charts*\n" +
                $"ChartEx [${gem.Symbol}](https://chartex.pro/?symbol=UNISWAP:{gem.Symbol}) |" +
                $" [${gem.Symbol}/$WETH](https://chartex.pro/?symbol=UNISWAP:{gem.Symbol}/WETH)\n\n"
                :
                string.Empty;

            var warningAfterDelete
                = gem.Recently == TokenAction.DELETED
                ?
                $"🛑 *WARNING*\n" +
                $"Address: [{gem.Id}](https://etherscan.io/token/{gem.Id}) \n\n" +
                $"Make sure to delete the allowance using fe. [revoke.cash](https://revoke.cash)\n\n"
                :
                string.Empty;

            var result =
                $"{RecentlyEmoji(gem)} *{gem.Recently.GetDescription()}* - Uniswap (v2)\n\n" +
                $"🦄 Uniswap\n" +
                $"💎 Token: *{gem.Name}*\n" +
                $"🚨 Symbol: *{gem.Symbol}*\n\n" +
                networkEffectVisible +
                statisticsVisible +
                chartsVisible +
                warningAfterDelete;

            return result;
        }
        public static Tuple<IReplyMarkup, string> ForPremiumTelegram(
            Gem gem,
            UniswapTokenDataResponse uniResponse,
            EthPlorerTokenInfoResponse ethPlorerTokenInfoResponse,
            EtherScanResponse etherScanResponse,
            UniswapPairDataResponse pairResponse,
            EthPlorerTopHoldersResponse ethPlorerTopHoldersResponse
            )
        {
            string tokenInfo = string.Empty;

            if (gem.Recently == TokenAction.ADDED)
            {
                tokenInfo = $"🥇 *Token - ${gem.Symbol}*\n";

                if (uniResponse.Success)
                {
                    tokenInfo +=
                        $"Initial Price: _${uniResponse.TokenData.Price} USD_\n" +
                        $"Txns: _{uniResponse.TokenData.DailyTxns}_\n\n";
                }
                else
                    tokenInfo += $"`Data unavailable` \n\n";
            }

            string tokenInfoDetails = string.Empty;

            if (gem.Recently == TokenAction.ADDED)
            {
                tokenInfoDetails += $"🥇 *Token Details*\n";

                if (ethPlorerTokenInfoResponse.Success)
                {
                    var dec = Convert.ToInt32(ethPlorerTokenInfoResponse.TokenInfo.Decimals);
                    var val = BigInteger.Parse(ethPlorerTokenInfoResponse.TokenInfo.TotalSupply);

                    tokenInfoDetails +=
                        $"Transfers: _{ethPlorerTokenInfoResponse.TokenInfo.TransfersCount}_\n" +
                        $"Owner: [{ethPlorerTokenInfoResponse.TokenInfo.Owner}](https://etherscan.io/address/{gem.Id})\n" +
                        $"Total supply: _{UnitConversion.Convert.FromWei(val, dec)}_ {gem.Symbol} \n\n";
                }
                else
                    tokenInfoDetails += $"`Data unavailable` \n\n";
            }

            string liquidityInfo = string.Empty;

            if (gem.Recently == TokenAction.ADDED)
            {
                liquidityInfo = $"🥈 *Liquidity*\n";

                if (uniResponse.Success)
                {
                    liquidityInfo +=
                        $"USD: _${uniResponse.TokenData.LiquidityUSD}_\n" +
                        $"ETH: _{uniResponse.TokenData.LiquidityETH}_\n" +
                        $"{gem.Symbol}: _{uniResponse.TokenData.LiquidityToken}_\n\n";
                }
                else
                    liquidityInfo += $"`Data unavailable` \n\n";
            }

            string contractInfo = string.Empty;

            if (gem.Recently == TokenAction.ADDED)
            {
                if (etherScanResponse.Success)
                {
                    contractInfo = etherScanResponse.Contract.IsVerified
                        ? $"✅ [Contract](https://etherscan.io/address/{gem.Id}) is verified \n\n"
                        : $"❌ [Contract](https://etherscan.io/address/{gem.Id}) is NOT verified \n\n";
                }
                else
                    contractInfo += $"`Data unavailable` \n\n";
            }

            var formPair = string.Empty;

            if (gem.Recently == TokenAction.ADDED)
            {
                formPair = $"🥉 *Pairs*\n";

                if (pairResponse.Success)
                {
                    foreach (var item in pairResponse.Pairs)
                    {
                        formPair +=
                            $"`{item.Token0.Symbol}/{item.Token1.Symbol}`\n" +
                            $"Total value: _${item.TotalLiquidityUSD} USD_\n" +
                            $"Created at: _{item.CreatedAt:dd.MM.yyyy}_\n" +
                            $"[Uniswap](https://info.uniswap.org/pair/{item.Id}) |" +
                            $" [DEXT](https://www.dextools.io/app/uniswap/pair-explorer/{item.Id}) |" +
                            $" [Astro](https://app.astrotools.io/pair-explorer/{item.Id}) |" +
                            $" [UniCrypt](https://v2.unicrypt.network/pair/{item.Id})" +
                            $"\n\n";
                    }
                }
                else
                    formPair += $"`Data unavailable` \n\n";
            }

            string topHoldersInfo = string.Empty;

            if (gem.Recently == TokenAction.ADDED)
            {
                if (ethPlorerTokenInfoResponse.Success)
                {
                    topHoldersInfo += $"🌐 *Current hodlers*: {ethPlorerTokenInfoResponse.TokenInfo.HoldersCount}.\n";
                }
                else
                {
                    topHoldersInfo += $"🌐 *Current hodlers*\n";
                }

                if (ethPlorerTopHoldersResponse.Success)
                {
                    topHoldersInfo += $"Top 5 below:\n";

                    var counter = 1;

                    foreach (var item in ethPlorerTopHoldersResponse.HolderList.Holders)
                    {
                        topHoldersInfo += $"[Hodler - {counter++}](https://etherscan.io/token/{gem.Id}?a={item.Address}) - `{item.Share}%` \n";
                    }

                    topHoldersInfo += $"\n\n";
                }
                else
                    topHoldersInfo += $"`Data unavailable` \n\n";
            }

            var banner =
                Content(gem) +
                tokenInfo +
                tokenInfoDetails +
                liquidityInfo +
                contractInfo +
                formPair +
                topHoldersInfo;

            var buttons = new InlineKeyboardMarkup(new[]
            {
                UniswapButtons(gem),
                EtherscanButtons(gem),
                new []
                {
                    InlineKeyboardButton.WithUrl("📧 Support", $"https://t.me/tkowalczyk")
                }
            });

            return new Tuple<IReplyMarkup, string>(buttons, banner);
        }
        public static Tuple<IReplyMarkup, string> ForFreeTelegram(Gem gem)
        {
            var banner =
                Content(gem) +
                $"👨‍👦‍👦 Our community:\n" +
                $"Chat - @GemTrackerCommunity\n" +
                $"Info - @GemTrackerAnnouncements\n\n" +
                $"📣 Ask for premium access and get:\n" +
                $"- info about *price, liquidity, contract, holders, txns, swaps*\n" +
                $"- links to *DEXT, Astro and UniCrypt*\n" +
                $"- insights about valueable gem or shitty scam";

            var buttons = new InlineKeyboardMarkup(new[]
            {
                UniswapButtons(gem),
                EtherscanButtons(gem),
                new []
                {
                    InlineKeyboardButton.WithUrl("⚙️ How to use?", $"https://gemtracker.club/#howtouse"),
                    InlineKeyboardButton.WithUrl("💰 Premium access?", $"https://gemtracker.club/#premium")
                },
                new []
                {
                    InlineKeyboardButton.WithUrl("📧 Author", $"https://twitter.com/tomkowalczyk")
                }
            });
            return new Tuple<IReplyMarkup, string>(buttons, banner);
        }
    }
}