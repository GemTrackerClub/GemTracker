using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IKyberService
    {
        Task<ListServiceResponse<KyberToken>> FetchAllAsync();
    }

    public class KyberService : IKyberService
    {
        private readonly string _baseUrl = "https://api.kyber.network/";
        public async Task<ListServiceResponse<KyberToken>> FetchAllAsync()
        {
            var result = new ListServiceResponse<KyberToken>();
            try
            {
                using var client = new WebClient();
                string httpApiResult = await client.DownloadStringTaskAsync($"{_baseUrl}currencies?include_delisted=true");

                if (!string.IsNullOrWhiteSpace(httpApiResult))
                {
                    var response = JsonSerializer.Deserialize<KyberTokenList>(httpApiResult);

                    if ((response is null) && !response.Error)
                    {
                        result.ListResponse = response.Data;
                    }
                    else
                    {
                        result.Message = $"Response error";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
    }
}