using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IKyberService
    {
        Task<KyberTokensResponse> FetchAllAsync();
    }

    public class KyberService : IKyberService
    {
        private readonly string _baseUrl = "https://api.kyber.network/";
        public async Task<KyberTokensResponse> FetchAllAsync()
        {
            var result = new KyberTokensResponse();
            try
            {
                using var client = new WebClient();
                string httpApiResult = await client.DownloadStringTaskAsync($"{_baseUrl}currencies?include_delisted=true");

                if (!string.IsNullOrWhiteSpace(httpApiResult))
                {
                    result.List = JsonSerializer.Deserialize<KyberTokenList>(httpApiResult);
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