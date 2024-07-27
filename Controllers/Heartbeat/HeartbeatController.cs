using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers.Heartbeat
{
    [ApiController]
    [Route("[controller]")]
    public class HeartbeatController(ILogger<HeartbeatController> logger) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var currentTime = DateTime.UtcNow;
            logger.LogInformation("Heartbeat check received at {Time}", currentTime);
            
            return Ok(new
            {
                Status = "Healthy",
                Time = currentTime
            });
        }
        
        [HttpGet("Exception")]
        public IActionResult GetException()
        {
            throw new Exception("Heartbeat check exception");
        }
    }
}