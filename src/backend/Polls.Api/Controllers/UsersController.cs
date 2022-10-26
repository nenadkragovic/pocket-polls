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
        var userResult = await _repository.RegisterUserAsync(userRegistration, Lib.Enums.Role.User);
        return !userResult.Succeeded ? new BadRequestObjectResult(userResult) : StatusCode((int)HttpStatusCode.Created);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
    {
        var result = await _repository.ValidateUserAsync(user);
        if (!result.Item1)
            return Unauthorized();
        return Ok(new TokenDto { Token = result.Item2 });
    }
}
