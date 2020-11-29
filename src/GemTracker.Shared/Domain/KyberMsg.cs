using GemTracker.Shared.Converters;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Numerics;
using Telegram.Bot.Types.ReplyMarkups;

namespace GemTracker.Shared.Domain
{
    public class KyberMsg
    {
        public static Tuple<IReplyMarkup, string> ForPremiumTelegram(
            Gem gem,
            EthPlorerTokenInfoResponse ethPlorerTokenInfoResponse,
            EtherScanResponse etherScanResponse,
            EthPlorerTopHoldersResponse ethPlorerTopHoldersResponse
            )
        {
            string tokenInfoDetails = string.Empty;

            if (gem.Recently == TokenActionType.KYBER_ADDED_TO_ACTIVE)
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

            string contractInfo = string.Empty;

            if (gem.Recently == TokenActionType.KYBER_ADDED_TO_ACTIVE)
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

            string topHoldersInfo = string.Empty;

            if (gem.Recently == TokenActionType.KYBER_ADDED_TO_ACTIVE)
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
                tokenInfoDetails +
                contractInfo +
                topHoldersInfo;

            var buttons = new InlineKeyboardMarkup(new[]
            {
                KyberButtons(gem),
                SharedMessageContent.EtherscanButtons(gem.Id),
                new []
                {
                    InlineKeyboardButton.WithUrl("📧 Support", $"https://t.me/GemTrackerCommunity")
                }
            });

            return new Tuple<IReplyMarkup, string>(buttons, banner);
        }

        public static string Content(Gem gem)
        {
            var networkEffectVisible = SharedMessageContent.NetworkEffectContent(gem.Recently, gem.Symbol, gem.Name);

            var statisticsVisible = SharedMessageContent.StatisticsContent(gem.Recently, gem.Id);

            var warningAfterDelete = SharedMessageContent.WarningContent(gem.Recently, gem.Id);

            var result =
                $"{DexType.KYBER.GetDescription().ToUpperInvariant()}\n" +
                $"{SharedMessageContent.RecentlyEmoji(gem.Recently)} *{gem.Recently.GetDescription()}*\n\n" +
                $"💎 Token: *{gem.Name}*\n" +
                $"🚨 Symbol: *{gem.Symbol}*\n\n" +
                networkEffectVisible +
                statisticsVisible +
                warningAfterDelete;

            return result;
        }

        private static InlineKeyboardButton[] KyberButtons(Gem gem)
        {
            var buyOrNot
                = gem.Recently == TokenActionType.KYBER_ADDED_TO_ACTIVE
                ? InlineKeyboardButton.WithUrl("📉 Buy", $"https://www.kyberswap.com/swap/eth-{gem.Symbol}")
                : InlineKeyboardButton.WithUrl("🛑 DON'T Buy it", $"https://www.kyberswap.com");

            return new[]
                {
                    InlineKeyboardButton.WithUrl("📋 Portfolio", $"https://www.kyberswap.com/portfolio"),
                    buyOrNot,
                    InlineKeyboardButton.WithUrl("📈 Transfer", $"https://www.kyberswap.com/transfer/{gem.Symbol}"),
                    InlineKeyboardButton.WithUrl("⏰ Limit", $"https://www.kyberswap.com/limit_order/{gem.Symbol}-weth"),
                };
        }
    }
}