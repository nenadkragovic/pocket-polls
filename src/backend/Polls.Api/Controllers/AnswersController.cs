using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Repositories;
using Serilog;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/answers")]
[Authorize]
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

        try
        {
            await _answersRepository.AddAnswers(Context.GetAdminId(), pollId, model);

            return Ok();
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);

            return BadRequest(e.Message);
        }

    }
}
