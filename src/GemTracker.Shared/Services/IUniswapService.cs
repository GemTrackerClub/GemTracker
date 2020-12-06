using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses.Generic;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IUniswapService
    {
        Task<ListServiceResponse<Token>> FetchAllAsync();
        Task<SingleServiceResponse<TokenData>> FetchTokenAsync(string tokenId);
        Task<ListServiceResponse<PairData>> FetchPairsAsync(string tokenId);
    }
    public class UniswapService : IUniswapService
    {
        private readonly IGraphQLClient _graphQLClient;
        public UniswapService()
        {
            _graphQLClient = new GraphQLHttpClient(
                "https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2",
                new NewtonsoftJsonSerializer());
        }
        public async Task<ListServiceResponse<Token>> FetchAllAsync()
        {
            var result = new ListServiceResponse<Token>();
            try
            {
                GraphQLResponse<TokenList> graphQLResponse = null;

                var list = new List<Token>();
                var last = "0x0";

                do
                {
                    var initialRequest = new GraphQLRequest
                    {
                        Query = @"
                    query GetTokens(
                      $first: Int,
	                    $orderBy: ID,
	                    $orderDirection: String) {
                        tokens(
                          first: $first, where: {
    		                    id_gt: " + $"\"{last}\"" + @"
                                },
                            orderBy: $orderBy,
    	                    orderDirection: $orderDirection) {
                                    id
                                    symbol
                                    name
                        }
                            }",
                        OperationName = "GetTokens",
                        Variables = new
                        {
                            first = 500
                        }
                    };

                    graphQLResponse = await _graphQLClient.SendQueryAsync<TokenList>(initialRequest);

                    if (!(graphQLResponse.Data is null))
                    {
                        if (graphQLResponse.Data.Tokens.AnyAndNotNull())
                        {
                            list.AddRange(graphQLResponse.Data.Tokens);

                            var lastOne = graphQLResponse.Data.Tokens.Last();

                            last = lastOne.Id;
                        }
                    }

                    if (!(graphQLResponse.Errors is null))
                    {
                        result.ListResponse = null;
                        result.Message = graphQLResponse.Errors?.FirstOrDefault().Message;
                        break;
                    }

                    Thread.Sleep(500);

                } while (!(graphQLResponse.Data is null) && (graphQLResponse.Errors is null) && graphQLResponse.Data.Tokens.AnyAndNotNull());

                result.ListResponse = list;
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }

        public async Task<ListServiceResponse<PairData>> FetchPairsAsync(string tokenId)
        {
            var result = new ListServiceResponse<PairData>();
            try
            {
                var pairDataRequest = new GraphQLRequest
                {
                    Query = @"
                    query GetPairData ($id: String, $first: Int){
                        pairs (
                          first: $first, 
                          orderBy: reserveUSD,
                          orderDirection: desc
                          where: 
                            {
                              token0: $id
                            })
                          {
                            id
                            reserveUSD
                            createdAtTimestamp
                            token0{
                              id
                              symbol
                              name
                            }
                            token1{
                              id
                              symbol
                              name
                            }
                          }
                    }",
                    OperationName = "GetPairData",
                    Variables = new
                    {
                        id = tokenId,
                        first = 5
                    }
                };

                var pairData = new List<PairData>();
                GraphQLResponse<PairList> pairDataResponse = null;

                pairDataResponse = await _graphQLClient.SendQueryAsync<PairList>(pairDataRequest);

                if (!(pairDataResponse.Data is null))
                {
                    if (pairDataResponse.Data.Pairs.AnyAndNotNull())
                    {
                        pairData.AddRange(pairDataResponse.Data.Pairs);
                    }
                }
                result.ListResponse = pairData;
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }

        public async Task<SingleServiceResponse<TokenData>> FetchTokenAsync(string tokenId)
        {
            var result = new SingleServiceResponse<TokenData>();
            try
            {
                var tokenDataRequest = new GraphQLRequest
                {
                    Query = @"
                query GetTokenData ($id: String, $first: Int){
                    tokenDayDatas(
                    first: $first,
                    orderBy: date,
                    orderDirection: desc,
                    where: { 
                      token: $id
                    }) {
                        token {
                        symbol
                        name
                        id
                    }
                    priceUSD
                    dailyTxns
                    totalLiquidityUSD
                    totalLiquidityETH
                    totalLiquidityToken
                    }
                }",
                    OperationName = "GetTokenData",
                    Variables = new
                    {
                        id = tokenId,
                        first = 1
                    }
                };

                TokenData tokenDayData = null;
                GraphQLResponse<TokenDataList> tokenDataResponse = null;

                tokenDataResponse = await _graphQLClient.SendQueryAsync<TokenDataList>(tokenDataRequest);

                if (!(tokenDataResponse.Data is null))
                {
                    if (tokenDataResponse.Data.TokenDayDatas.AnyAndNotNull())
                    {
                        tokenDayData = tokenDataResponse.Data.TokenDayDatas.FirstOrDefault();
                    }
                }

                result.ObjectResponse = tokenDayData;
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
    }
}