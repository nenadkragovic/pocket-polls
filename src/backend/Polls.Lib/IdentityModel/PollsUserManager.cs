using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polls.Lib.Database.Models;

namespace Polls.Lib.IdentityModel
{
    public class PollsUserManager : UserManager<User>
    {
        public PollsUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
                                 IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
                                 IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
                                 IdentityErrorDescriber errors, IServiceProvider services, ILogger<PollsUserManager> logger)
                                 :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        { }
    }
}
