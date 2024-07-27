using System.Net;
using HackerNewsApi.Clients.HackerNews;
using HackerNewsApi.Entities.HackerNews;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;

namespace HackerNewsTest.Clients.HackerNews
{
    [TestClass]
    public class HackerNewsClientTests
    {
        private HttpClient _httpClient;
        private ILogger<HackerNewsClient> _logger;
        private HackerNewsClient _hackerNewsClient;
        private MockHttpMessageHandler _mockHttpMessageHandler;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttpMessageHandler);
            _logger = Substitute.For<ILogger<HackerNewsClient>>();
            _hackerNewsClient = new HackerNewsClient(_httpClient, _logger);
        }

        [TestMethod]
        public async Task GetTopStoriesAsync_ShouldReturnListOfStoryIds()
        {
            var expectedIds = new List<int> { 1, 2, 3, 4, 5 };
            var jsonResponse = JsonConvert.SerializeObject(expectedIds);

            _mockHttpMessageHandler.SetupResponse(HttpStatusCode.OK, jsonResponse);

            var result = await _hackerNewsClient.GetTopStoriesAsync();

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expectedIds, result);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetTopStoriesAsync_ShouldThrowException_OnFailure()
        {
            _mockHttpMessageHandler.SetupResponse(HttpStatusCode.InternalServerError, string.Empty);

            await _hackerNewsClient.GetTopStoriesAsync();
        }

        [TestMethod]
        public async Task GetStoryByIdAsync_ShouldReturnStory()
        {
            var storyId = 1;
            var expectedStory = new HackerNewsStory { Id = storyId, Title = "Test Story" };
            var jsonResponse = JsonConvert.SerializeObject(expectedStory);

            _mockHttpMessageHandler.SetupResponse(HttpStatusCode.OK, jsonResponse);

            var result = await _hackerNewsClient.GetStoryByIdAsync(storyId);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedStory.Id, result.Id);
            Assert.AreEqual(expectedStory.Title, result.Title);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetStoryByIdAsync_ShouldThrowException_OnFailure()
        {
            var storyId = 1;

            _mockHttpMessageHandler.SetupResponse(HttpStatusCode.InternalServerError, string.Empty);

            await _hackerNewsClient.GetStoryByIdAsync(storyId);
        }
    }

    public class MockHttpMessageHandler : DelegatingHandler
    {
        private HttpResponseMessage? _responseMessage;

        public void SetupResponse(HttpStatusCode statusCode, string content)
        {
            _responseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseMessage ?? new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }
    }
}
