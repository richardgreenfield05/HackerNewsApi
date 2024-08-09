
using Microsoft.AspNetCore.Mvc.Testing;

namespace HackerNewsTest.Controllers.HackerNews
{
    [TestClass]
    [TestCategory("Integration")]
    public class HackerNewsControllerTests
    {
        private readonly HttpClient _client;

        public HackerNewsControllerTests()
        {
            WebApplicationFactory<Program> factory = new();
            _client = factory.CreateClient();
        }

        [TestMethod]
        public async Task GetTopStories_ShouldReturnOkResponse_WithTopStories()
        {
            var response = await _client.GetAsync("/api/hackernews/stories");
            var content = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content));
        }

        [TestMethod]
        public async Task GetStoryById_ShouldReturnOkResponse_WithStory()
        {
            int storyId = 1;
            var response = await _client.GetAsync($"/api/hackernews/story/{storyId}");
            var content = await response.Content.ReadAsStringAsync();
            
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content));
        }

        [TestMethod]
        public async Task GetStoryById_ShouldReturnNotFound_ForInvalidId()
        {
            int invalidStoryId = -1;
            var response = await _client.GetAsync($"/api/hackernews/story/{invalidStoryId}");

            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
