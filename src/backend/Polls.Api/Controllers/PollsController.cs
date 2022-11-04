using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polls.Lib.MessageBrokers;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Exceptions;
using Polls.Lib.Repositories;
using Polls.Lib.Repositories.Authentication;
using System.Security.Claims;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/polls")]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly PollsRepository _pollsRepository;
    private readonly IUserAuthenticationRepository _userAuthenticationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PushNotificationsBroker _pushNotificationsService;

    public PollsController(PollsRepository pollsRepository,
                           PushNotificationsBroker pushNotificationsService,
                           IUserAuthenticationRepository userAuthenticationRepository,
                           IHttpContextAccessor httpContextAccessor)
    {
        _pollsRepository = pollsRepository;
        _userAuthenticationRepository = userAuthenticationRepository;
        _httpContextAccessor = httpContextAccessor;
        _pushNotificationsService = pushNotificationsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<ListPollsDto>), 200)]
    [ProducesResponseType(204)]
    [AllowAnonymous]
    public async Task<IActionResult> List(
        [FromQuery] int offset = 0,
        [FromQuery] byte limit = 10,
        [FromQuery] bool getForUser = false,
        [FromQuery] string? searchParam = "")
    {
        Guid userId = Guid.Empty;
        if (getForUser)
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userAuthenticationRepository.GetUserByName(username);
            userId = user.Id;
        }

        var result = await _pollsRepository.ListPolls(offset, limit, userId, searchParam ?? "");

        if (result == null || result.TotalRecords == 0)
            return NoContent();

        return Ok(result);
    }



    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Poll), 200)]
    [ProducesResponseType(204)]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var result = await _pollsRepository.GetPollById(id);

        if (result != null)
        {
            _pushNotificationsService.PublishMessage(new BrokerMessage()
            {
                Title = "Polls listed on device.",
                Message = "Someone listed polls"
            });

            return Ok(result);
        }

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Poll), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        var user = await _userAuthenticationRepository.GetUserByName(username);

        var poll = await _pollsRepository.AddPoll(user.Id, model);

        return Created($"polls/{poll?.Id}", poll);

    }

    [HttpPost("{pollId}/questions")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddQuestions([FromRoute] long pollId, [FromBody] ICollection<CreateQuestionDto> questions)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        await _pollsRepository.AddQuestions(pollId, questions);

        return Ok();
    }

    [HttpDelete("{pollId}/questions")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteQuestions([FromRoute] long pollId, [FromBody] ICollection<DeleteQuestionDto> questionsToDelete)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        try
        {
            await _pollsRepository.DeleteQuestions(pollId, questionsToDelete);

            return Ok();
        }
        catch (RecordNotFoundException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{pollId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeletePoll([FromRoute] long pollId)
    {
        try
        {
            await _pollsRepository.DeletePoll(pollId);

            return Ok();
        }
        catch (RecordNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
