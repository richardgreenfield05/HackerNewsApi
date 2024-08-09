using HackerNewsApi.Entities.HackerNews;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace HackerNewsApi.Clients.HackerNews
{
    public class HackerNewsClient
    {
        private readonly HttpClient _client;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly ILogger<HackerNewsClient> _logger;

        public HackerNewsClient(HttpClient client, ILogger<HackerNewsClient> logger)
        {
            _client = client;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(3, (exception, retryCount) =>
                {
                    _logger.LogWarning($"Retrying due to: {exception.Exception?.Message}. Retry count: {retryCount}");
                });
        }

        public async Task<List<int>?> GetTopStoriesAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _client.GetAsync($"topstories.json"));

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<int>>(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HttpRequestException: Failed to get top stories.");
                throw;
            }
            finally
            {
                _logger.LogInformation("Completed GetTopStoriesAsync operation.");
            }
        }

        public async Task<HackerNewsStory?> GetStoryByIdAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _client.GetAsync($"item/{id}.json"));

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HackerNewsStory>(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HttpRequestException: Failed to get story with ID: {id}.");
                throw;
            }
            finally
            {
                _logger.LogInformation($"Completed GetStoryByIdAsync operation for story ID: {id}.");
            }
        }
    }
}
