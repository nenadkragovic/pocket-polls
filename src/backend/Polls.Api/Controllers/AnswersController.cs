using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.MessageBrokers;
using Polls.Lib.Repositories;
using Polls.Lib.Repositories.Authentication;
using Serilog;
using System.Security.Claims;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/answers")]
[Authorize]
public class AnswersController : ControllerBase
{
    private readonly AnswersRepository _answersRepository;
    private readonly IUserAuthenticationRepository _userAuthenticationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PushNotificationsBroker _pushNotificationsService;


    public AnswersController(AnswersRepository answersRepository,
                             PushNotificationsBroker pushNotificationsService,
                             IUserAuthenticationRepository userAuthenticationRepository,
                             IHttpContextAccessor httpContextAccessor)
    {
        _answersRepository = answersRepository;
        _pushNotificationsService = pushNotificationsService;
        _httpContextAccessor = httpContextAccessor;
        _userAuthenticationRepository = userAuthenticationRepository;
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
            var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userAuthenticationRepository.GetUserByName(username);

            var ownerId = await _answersRepository.AddAnswers(user.Id, pollId, model);

            _pushNotificationsService.PublishMessage(new BrokerMessage()
            {
                Title = $"New answer!",
                Message = "Someone just answered your poll.",
                SendToAll = false,
                UserId = ownerId,
            });

            return Ok();
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);

            return BadRequest(e.Message);
        }

    }
}
