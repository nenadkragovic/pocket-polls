using Microsoft.AspNetCore.Identity;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Enums;

namespace Polls.Lib.Repositories.Authentication
{
    public interface IUserAuthenticationRepository
    {
        Task<IdentityResult> RegisterUserAsync(CreateUserDto userForRegistration, Role role);

        /// <summary>
        /// Validate user and create token
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>
        ///     First tuple item is validation result. If used is valid second tuple item contains token
        /// </returns>
        Task<Tuple<bool, string>> ValidateUserAsync(UserLoginDto loginDto);
        Task<User> GetUserByName(string username);
        Task<User> GetUserById(Guid userId);

    }
}
