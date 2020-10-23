using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using GraphQL;
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
        Task<UniswapResponse> FetchAllAsync();
    }
    public class UniswapService : IUniswapService
    {
        public async Task<UniswapResponse> FetchAllAsync()
        {
            var result = new UniswapResponse();
            try
            {
                using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());

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

                    graphQLResponse = await graphQLClient.SendQueryAsync<TokenList>(initialRequest);

                    if (!(graphQLResponse.Data is null))
                    {
                        if (graphQLResponse.Data.Tokens.AnyAndNotNull())
                        {
                            list.AddRange(graphQLResponse.Data.Tokens);

                            var lastOne = graphQLResponse.Data.Tokens.Last();

                            last = lastOne.Id;
                        }
                    }

                    if(!(graphQLResponse.Errors is null))
                    {
                        result.Tokens = null;
                        result.Message = graphQLResponse.Errors?.FirstOrDefault().Message;
                        break;
                    }

                    Thread.Sleep(500);

                } while (!(graphQLResponse.Data is null) && (graphQLResponse.Errors is null) && graphQLResponse.Data.Tokens.AnyAndNotNull());

                result.Tokens = list;
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
    }
}