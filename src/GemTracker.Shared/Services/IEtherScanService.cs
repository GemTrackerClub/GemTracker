using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IEtherScanService
    {
        Task<SingleServiceResponse<SmartContract>> IsSmartContractVerifiedAsync(string contractAddress);
    }

    public class EtherScanService : IEtherScanService
    {
        private readonly string _baseUrl = "https://api.etherscan.io/api?";
        private readonly string _apiKey;
        public EtherScanService(
            string apiKey)
        {
            _apiKey = apiKey;
        }
        public async Task<SingleServiceResponse<SmartContract>> IsSmartContractVerifiedAsync(string contractAddress)
        {
            var result = new SingleServiceResponse<SmartContract>();
            var parameters = new Dictionary<string, object>()
            {
                {"module", "contract" },
                {"action", "getabi" },
                {"address", contractAddress }
            };

            try
            {
                using var client = new WebClient();
                string httpApiResult = await client.DownloadStringTaskAsync(ConstructRequest(parameters));

                result.ObjectResponse = JsonSerializer.Deserialize<SmartContract>(httpApiResult);
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
        private string ConstructRequest(Dictionary<string, object> parameters)
        {
            parameters.Add("apiKey", _apiKey);
            string requestUrl = _baseUrl + string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
            return requestUrl;
        }
    }
}