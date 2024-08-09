using HackerNewsApi.Caching;
using HackerNewsApi.Clients.HackerNews;
using HackerNewsApi.Entities.HackerNews;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers.HackerNews
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController(HackerNewsClient hackerNewsClient)
        : ControllerBase
    {
        [HttpGet("stories")]
        public async Task<IActionResult> GetTopStories()
        {
            List<int>? stories = await hackerNewsClient.GetTopStoriesAsync();
            if (stories == null)
            {
                return NotFound("No top stories found.");
            }
            return Ok(stories);
        }

        [HttpGet("story/{id}")]
        [CacheControl(1)]
        public async Task<IActionResult> GetStoryById(int id)
        {
            HackerNewsStory? story = await hackerNewsClient.GetStoryByIdAsync(id);
            if (story == null)
            {
                return NotFound($"Story with ID {id} not found.");
            }
            return Ok(story);
        }
    }
}
