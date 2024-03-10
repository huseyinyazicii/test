using Microsoft.AspNetCore.Mvc;

namespace QueueDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly INameQueueService _nameQueueService;
    private readonly IBackgroundTaskQueue<string> _queue;

    public TestController(INameQueueService nameQueueService, IBackgroundTaskQueue<string> queue)
    {
        _nameQueueService = nameQueueService;
        _queue = queue;
    }

    [HttpPost("demo1")]
    public IActionResult AddQueue1(string[] names)
    {
        foreach (var name in names) 
            _nameQueueService.Add(name);

        return Ok();
    }

    [HttpPost("demo2")]
    public async Task<IActionResult> AddQueue2(string[] names)
    {
        foreach (var name in names)
            await _queue.AddQueue(name);

        return Ok();
    }
}
