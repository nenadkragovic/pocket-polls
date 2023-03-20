using Microsoft.AspNetCore.Mvc;
using Polls.Lib.DTO;
using Polls.Lib.Repositories.Authentication;
using System.Net;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserAuthenticationRepository _repository;

    public UsersController(IUserAuthenticationRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterRespondent([FromBody] CreateUserDto userRegistration)
    {
        if (!ModelState.IsValid)
        {
            BadRequest(ModelState);
        }

        var userResult = await _repository.RegisterUserAsync(userRegistration, Lib.Enums.Role.User);
        return !userResult.Succeeded ? new BadRequestObjectResult(userResult) : StatusCode((int)HttpStatusCode.Created);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
    {
        var result = await _repository.ValidateUserAsync(user);
        if (!result.Authorized)
            return Unauthorized(result);

        return Ok(new TokenDto { Token = result.Token, UserId = result.UserId });
    }
}
