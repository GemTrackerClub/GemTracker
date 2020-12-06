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
        Task<SingleServiceResponse<KyberTokenList>> FetchAllAsync();
    }

    public class KyberService : IKyberService
    {
        private readonly string _baseUrl = "https://api.kyber.network/";
        public async Task<SingleServiceResponse<KyberTokenList>> FetchAllAsync()
        {
            var result = new SingleServiceResponse<KyberTokenList>();
            try
            {
                using var client = new WebClient();
                string httpApiResult = await client.DownloadStringTaskAsync($"{_baseUrl}currencies?include_delisted=true");

                if (!string.IsNullOrWhiteSpace(httpApiResult))
                {
                    result.ObjectResponse = JsonSerializer.Deserialize<KyberTokenList>(httpApiResult);
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