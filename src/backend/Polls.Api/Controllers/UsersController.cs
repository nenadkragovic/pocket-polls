using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;

namespace Polls.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            //await _signInManager.SignInAsync(user, isPersistent: false);

            return Created($"api/users/{user.Id}", user);
        }

        return BadRequest();

    }
}
