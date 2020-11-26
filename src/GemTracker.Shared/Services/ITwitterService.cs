using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses;
using System;
using System.Threading.Tasks;
using Tweetinvi;

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
        private readonly string _accessToken;
        private readonly string _accessSecret;
        private bool IsActive
            => !string.IsNullOrWhiteSpace(_apiKey) && !string.IsNullOrWhiteSpace(_apiSecret)
            && !string.IsNullOrWhiteSpace(_accessToken) && !string.IsNullOrWhiteSpace(_accessSecret);
        public TwitterService(
            string apiKey,
            string apiSecret,
            string accessToken,
            string accessSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _accessToken = accessToken;
            _accessSecret = accessSecret;
        }
        public async Task<SocialResponse> SendMessageAsync(string message)
        {
            var response = new SocialResponse();
            try
            {
                if (IsActive)
                {
                    var userClient = new TwitterClient(_apiKey, _apiSecret, _accessToken, _accessSecret);

                    var tweet = await userClient.Tweets.PublishTweetAsync(message);
                }

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