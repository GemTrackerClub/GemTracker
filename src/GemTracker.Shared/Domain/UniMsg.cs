using GemTracker.Shared.Converters;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Domain
{
    public class UniMsg
    {
        public static string ForTwitterSummary(IEnumerable<Gem> gems, TokenActionType tokenAction, int interval)
            => $"🦄 Uniswap (for last {interval / 60} h) \n\n" +
                $" {SharedMessageContent.RecentlyEmoji(tokenAction)} {tokenAction.GetDescription()} \n\n" +
                $"💎 {gems.Count()} Tokens\n" +
                $"🚨 Some of them: {string.Join(" ", gems.Take(5).Select(g => $"${g.Symbol}"))}\n\n" +
                $"Join for free: https://t.me/GemTrackerClub \n" +
                $"💰 Ask for premium: https://gemtracker.club/#premium \n" +
                $"( $BTC $ETH $ALTS $UNI #uniswap #cryptocurrency #gem #gemtrackerclub )";

        private static InlineKeyboardButton[] UniswapButtons(Gem gem)
        {
            var buyOrNot
                = gem.Recently == TokenActionType.ADDED
                ? InlineKeyboardButton.WithUrl("📉 Buy", $"https://app.uniswap.org/#/swap?outputCurrency={gem.Id}")
                : InlineKeyboardButton.WithUrl("🛑 DON'T Buy it", $"https://uniswap.info/");

            return new[]
                {
                    InlineKeyboardButton.WithUrl("📋 Info", $"https://uniswap.info/token/{gem.Id}"),
                    buyOrNot,
                    InlineKeyboardButton.WithUrl("📈 Sell", $"https://app.uniswap.org/#/swap?inputCurrency={gem.Id}"),
                };
        }

        private static string Content(Gem gem)
        {
            var networkEffectVisible = SharedMessageContent.NetworkEffectContent(gem.Recently, gem.Symbol, gem.Name);

            var statisticsVisible = SharedMessageContent.StatisticsContent(gem.Recently, gem.Id);

            var chartsVisible
                = gem.Recently == TokenActionType.ADDED
                ?
                $"📊 *Charts*\n" +
                $"ChartEx [${gem.Symbol}](https://chartex.pro/?symbol=UNISWAP:{gem.Symbol}) |" +
                $" [${gem.Symbol}/$WETH](https://chartex.pro/?symbol=UNISWAP:{gem.Symbol}/WETH)\n\n"
                :
                string.Empty;

            var warningAfterDelete = SharedMessageContent.WarningContent(gem.Recently, gem.Id);

            var result =
                $"{DexType.UNISWAP.GetDescription().ToUpperInvariant()}\n" +
                $"{SharedMessageContent.RecentlyEmoji(gem.Recently)} *{gem.Recently.GetDescription()}*\n\n" +
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

            if (gem.Recently == TokenActionType.ADDED)
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

            if (gem.Recently == TokenActionType.ADDED)
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

            if (gem.Recently == TokenActionType.ADDED)
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

            if (gem.Recently == TokenActionType.ADDED)
            {
                if (etherScanResponse.Success)
                {
                    contractInfo = etherScanResponse.Contract.IsVerified
                        ? $"✅ [Contract](https://etherscan.io/address/{gem.Id}#code) is verified \n\n"
                        : $"❌ [Contract](https://etherscan.io/address/{gem.Id}#code) is NOT verified \n\n";
                }
                else
                    contractInfo += $"`Data unavailable` \n\n";
            }

            var formPair = string.Empty;

            if (gem.Recently == TokenActionType.ADDED)
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

            if (gem.Recently == TokenActionType.ADDED)
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
                SharedMessageContent.EtherscanButtons(gem.Id),
                new []
                {
                    InlineKeyboardButton.WithUrl("📧 Support", $"https://t.me/GemTrackerCommunity")
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
                SharedMessageContent.EtherscanButtons(gem.Id),
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