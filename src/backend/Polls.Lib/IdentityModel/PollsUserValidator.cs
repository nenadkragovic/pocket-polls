using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Polls.Lib.Database.Models;

namespace Polls.Lib.IdentityModel
{
    public class PollsUserValidator<TUser> : UserValidator<TUser> where TUser : User
    {
        public PollsUserValidator(IOptions<IdentityOptions> optionsAccessor, IdentityErrorDescriber errors = null)
        {
            Options = optionsAccessor?.Value ?? new IdentityOptions();
            Describer = errors ?? new IdentityErrorDescriber();
        }

        protected internal IdentityOptions Options { get; set; }

        public new IdentityErrorDescriber Describer { get; private set; }

        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var errors = new List<IdentityError>();
            await ValidateUserName(manager, user, errors);

            if (Options.User.RequireUniqueEmail)
            {
                await ValidateEmail(manager, user, errors);
            }

            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task ValidateUserName(UserManager<TUser> manager, TUser user, ICollection<IdentityError> errors)
        {
            var userName = await manager.GetUserNameAsync(user);
            if (string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(Describer.InvalidUserName(userName));
            }
            else if (!string.IsNullOrEmpty(Options.User.AllowedUserNameCharacters) && userName.Any(c => !Options.User.AllowedUserNameCharacters.Contains(c)))
            {
                errors.Add(Describer.InvalidUserName(userName));
            }
            else
            {
                var owner = await manager.FindByNameAsync(userName);
                if (owner != null && !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
                {
                    errors.Add(Describer.DuplicateUserName(userName));
                }
            }
        }

        private async Task ValidateEmail(UserManager<TUser> manager, TUser user, List<IdentityError> errors)
        {
            var email = await manager.GetEmailAsync(user);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(Describer.InvalidEmail(email));
                return;
            }

            var owner = await manager.FindByEmailAsync(email);
            if (owner != null && !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
            {
                errors.Add(Describer.DuplicateEmail(email));
            }
        }
    }
}
