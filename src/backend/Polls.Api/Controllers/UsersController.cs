using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Repositories;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserStore _userStore;

    public UsersController(UserStore userStore)
    {
        _userStore = userStore;
    }

    //[HttpGet("{pollId}")]
    //[ProducesResponseType(typeof(Poll), 200)]
    //[ProducesResponseType(204)]
    //public async Task<IActionResult> Get([FromRoute] long pollId)
    //{
    //    var result = await _answersRepository.GetAnswersByPollId(pollId);

    //    if (result != null)
    //        return Ok(result);

    //    return NoContent();
    //}

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var result = await _userStore.CreateAsync(new User()
        {
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        });

        if (result.Succeeded)
            return Created("", null);

        return BadRequest();

    }
}
