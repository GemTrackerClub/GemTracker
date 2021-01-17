using GemTracker.Shared.Converters;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Numerics;
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

        public static string ChartsContent(TokenActionType tokenAction, string gemSymbol)
            => tokenAction == TokenActionType.ADDED
                ?
                    $"📊 *Charts*\n" +
                    $"ChartEx [${gemSymbol}](https://chartex.pro/?symbol=UNISWAP:{gemSymbol}) |" +
                    $" [${gemSymbol}/$WETH](https://chartex.pro/?symbol=UNISWAP:{gemSymbol}/WETH)\n\n"
                :
                    string.Empty;

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

        public static string TokenDataContent(TokenActionType tokenAction, string symbol, SingleServiceResponse<TokenData> tokenData)
        {
            string tokenInfo = string.Empty;

            if (tokenAction == TokenActionType.ADDED)
            {
                tokenInfo = $"🥇 *Token - ${symbol}*\n";

                if (tokenData.Success)
                {
                    tokenInfo +=
                        $"Initial Price: _${tokenData.ObjectResponse.Price} USD_\n" +
                        $"Txns: _{tokenData.ObjectResponse.DailyTxns}_\n\n";
                }
                else
                    tokenInfo += $"`Data unavailable` \n\n";
            }

            return tokenInfo;
        }

        public static string TokenDetailsContent(TokenActionType tokenAction, string symbol, SingleServiceResponse<TokenInfo> tokenInfo)
        {
            string tokenInfoDetails = string.Empty;

            if (tokenAction == TokenActionType.ADDED || tokenAction == TokenActionType.KYBER_ADDED_TO_ACTIVE)
            {
                tokenInfoDetails += $"🥇 *Token Details*\n";

                if (tokenInfo.Success)
                {
                    var dec = Convert.ToInt32(tokenInfo.ObjectResponse.Decimals);
                    var val = BigInteger.Parse(tokenInfo.ObjectResponse.TotalSupply);

                    var owner = string.IsNullOrWhiteSpace(tokenInfo.ObjectResponse.Owner)
                        ? $"Owner: `Data unavailable`\n"
                        : $"Owner: [{tokenInfo.ObjectResponse.Owner}](https://etherscan.io/address/{tokenInfo.ObjectResponse.Owner})\n";

                    tokenInfoDetails +=
                        $"Transfers: _{tokenInfo.ObjectResponse.TransfersCount}_\n" +
                        owner +
                        $"Total supply: _{UnitConversion.Convert.FromWei(val, dec)}_ {symbol} \n\n";
                }
                else
                    tokenInfoDetails += $"`Data unavailable` \n\n";
            }

            return tokenInfoDetails;
        }
    }
}