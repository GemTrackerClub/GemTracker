using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface ITwitterService
    {
        Task<SocialResponse> SendMessageAsync(string message);
    }

    public class TwitterService : ITwitterService
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _token;
        public TwitterService(
            string apiKey,
            string apiSecret,
            string token)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _token = token;
        }
        public async Task<SocialResponse> SendMessageAsync(string message)
        {
            var response = new SocialResponse();
            try
            {


                response.Success = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.GetFullMessage();
            }
            return response;
        }
    }
}