using Microsoft.AspNetCore.Mvc;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Exceptions;
using Polls.Lib.Repositories;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/answers")]
public class AnswersController : ControllerBase
{
    private readonly PollsRepository _pollsRepository;

    public AnswersController(PollsRepository pollsRepository)
    {
        _pollsRepository = pollsRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<LiustPollsDto>), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> List([FromQuery] int offset = 0, [FromQuery] byte limit = 10, [FromQuery] string? searchParam = "")
    {
        var result = await _pollsRepository.ListPolls(offset, limit, searchParam ?? "");

        if (result == null || result.TotalRecords == 0)
            return NoContent();

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Poll), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var result = await _pollsRepository.GetPollById(id);

        if (result != null)
            return Ok(result);

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Poll), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var poll = await _pollsRepository.AddPoll(model);

        return Created($"polls/{poll?.Id}", poll);

    }
}
