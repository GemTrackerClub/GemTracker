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

        public static string TokenAndLiquidityDataContent(TokenActionType tokenAction, string symbol, SingleServiceResponse<TokenData> tokenData)
        {
            string tokenInfo = string.Empty;
            string liquidityInfo = string.Empty;

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

                liquidityInfo = $"🥈 *Liquidity*\n";

                if (tokenData.Success)
                {
                    liquidityInfo +=
                        $"USD: _${tokenData.ObjectResponse.LiquidityUSD}_\n" +
                        $"ETH: _{tokenData.ObjectResponse.LiquidityETH}_\n" +
                        $"{symbol}: _{tokenData.ObjectResponse.LiquidityToken}_\n\n";
                }
                else
                    liquidityInfo += $"`Data unavailable` \n\n";
            }

            return tokenInfo + liquidityInfo;
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
                        $"Hodlers: {tokenInfo.ObjectResponse.HoldersCount}\n" +
                        owner +
                        $"Total supply: _{UnitConversion.Convert.FromWei(val, dec)}_ {symbol} \n\n";
                }
                else
                    tokenInfoDetails += $"`Data unavailable` \n\n";
            }

            return tokenInfoDetails;
        }

        public static string TokenContractContent(TokenActionType tokenAction, string tokenId, SingleServiceResponse<SmartContract> contract)
        {
            string contractInfo = string.Empty;

            if (tokenAction == TokenActionType.ADDED || tokenAction == TokenActionType.KYBER_ADDED_TO_ACTIVE)
            {
                if (contract.Success)
                {
                    contractInfo = contract.ObjectResponse.IsVerified
                        ? $"✅ [Contract](https://etherscan.io/address/{tokenId}#code) is verified \n\n"
                        : $"❌ [Contract](https://etherscan.io/address/{tokenId}#code) is NOT verified \n\n";
                }
                else
                    contractInfo += $"`Data unavailable` \n\n";
            }

            return contractInfo;
        }

        public static string TokenPairsContent(TokenActionType tokenAction, ListServiceResponse<PairData> pairResponse)
        {
            var formPair = string.Empty;

            if (tokenAction == TokenActionType.ADDED)
            {
                formPair = $"🥉 *Pairs*\n";

                if (pairResponse.Success)
                {
                    foreach (var item in pairResponse.ListResponse)
                    {
                        formPair +=
                            $"`{item.Token0.Symbol}/{item.Token1.Symbol}`\n" +
                            $"Total value: _${item.TotalLiquidityUSD} USD_\n" +
                            $"Created at: _{item.CreatedAt:dd.MM.yyyy}_\n" +
                            $"[Uniswap](https://info.uniswap.org/pair/{item.Id}) |" +
                            $" [DEXT](https://www.dextools.io/app/uniswap/pair-explorer/{item.Id}) |" +
                            $" [Astro](https://app.astrotools.io/pair-explorer/{item.Id}) |" +
                            $" [MoonTools](https://app.moontools.io/pairs/{item.Id}) |" +
                            $" [UniCrypt](https://v2.unicrypt.network/pair/{item.Id})" +
                            $"\n\n";
                    }
                }
                else
                    formPair += $"`Data unavailable` \n\n";
            }

            return formPair;
        }

        public static string TokenHoldersContent(TokenActionType tokenAction, string tokenId, int topHodlersNumber, SingleServiceResponse<TopHolderList> serviceResponse)
        {
            string topHoldersInfo = string.Empty;

            if (tokenAction == TokenActionType.ADDED || tokenAction == TokenActionType.KYBER_ADDED_TO_ACTIVE)
            {
                if (serviceResponse.Success)
                {
                    topHoldersInfo += $"🌐 Top {topHodlersNumber} hodlers below:\n";

                    var counter = 1;

                    foreach (var item in serviceResponse.ObjectResponse.Holders)
                    {
                        topHoldersInfo += $"[Hodler - {counter++}](https://etherscan.io/token/{tokenId}?a={item.Address}) - `{item.Share}%` \n";
                    }

                    topHoldersInfo += $"\n\n";
                }
                else
                    topHoldersInfo += $"`Data unavailable` \n\n";
            }

            return topHoldersInfo;
        }
    }
}