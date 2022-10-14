using Microsoft.AspNetCore.Mvc;
using Polls.Api.Database.Models;
using Polls.Api.DTO;
using Polls.Api.Repositories;

namespace Polls.Api.Controllers;

[ApiController]
[Route("polls")]
public class PollsController : ControllerBase
{
    private readonly ILogger<PollsController> _logger;
    private readonly PollsRepository _pollsRepository;

    public PollsController(ILogger<PollsController> logger, PollsRepository pollsRepository)
    {
        _logger = logger;
        _pollsRepository = pollsRepository;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int offset, [FromQuery] byte limit, [FromQuery] string searchParam)
    {
        var result = await _pollsRepository.ListPolls(offset, limit, searchParam);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var result = await _pollsRepository.GetPollById(id);

        if (result != null)
            return Ok(result);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var poll = await _pollsRepository.AddPoll(model);

        return Created($"polls/{poll?.Id}", poll);

    }
}
