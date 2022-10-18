using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;

namespace Polls.Lib.IdentityModel
{
    public class PollsUserStore : UserStore<User, Role, Context, string>
    {
        public PollsUserStore(Context context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {

        }
    }
}
