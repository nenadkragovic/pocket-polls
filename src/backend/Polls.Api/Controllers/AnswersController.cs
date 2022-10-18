using Microsoft.AspNetCore.Mvc;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Repositories;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/answers")]
public class AnswersController : ControllerBase
{
    private readonly AnswersRepository _answersRepository;

    public AnswersController(AnswersRepository answersRepository)
    {
        _answersRepository = answersRepository;
    }

    [HttpGet("{pollId}")]
    [ProducesResponseType(typeof(Poll), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Get([FromRoute] long pollId)
    {
        var result = await _answersRepository.GetAnswersByPollId(pollId);

        if (result != null)
            return Ok(result);

        return NoContent();
    }

    [HttpPost("{pollId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddAnswers([FromRoute] long pollId, [FromBody] AddAnswersDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        await _answersRepository.AddAnswers("1efc5e3a-283b-4b05-b1ea-d2cd424c59d4", pollId , model);

        return Ok();

    }
}
