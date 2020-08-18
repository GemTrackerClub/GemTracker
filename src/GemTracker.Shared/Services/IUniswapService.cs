using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IUniswapService
    {
        Task<UniswapResponse> FetchAllAsync(int maxSize = 5000);
    }
    public class UniswapService : IUniswapService
    {
        public async Task<UniswapResponse> FetchAllAsync(int maxSize = 5000)
        {
            var result = new UniswapResponse();
            try
            {
                using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());

                var list = new List<Token>();

                for (int i = 0; i < maxSize; i += 500)
                {
                    var request = new GraphQLRequest
                    {
                        Query = @"
                            query GetTokens($first: Int, $skip: Int) {
                              tokens(first: $first, skip: $skip) {
                                id
                                symbol
                                name
                              }
                            }",
                        OperationName = "GetTokens",
                        Variables = new
                        {
                            first = 500,
                            skip = i
                        }
                    };

                    var graphQLResponse = await graphQLClient.SendQueryAsync<TokenList>(request);

                    if (!(graphQLResponse.Data is null))
                    {
                        if (graphQLResponse.Data.Tokens.AnyAndNotNull())
                        {
                            list.AddRange(graphQLResponse.Data.Tokens);
                        }
                    }
                    Thread.Sleep(2000);
                }
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