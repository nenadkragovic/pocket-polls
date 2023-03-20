using Microsoft.AspNetCore.Identity;
using Polls.Lib.Enums;

namespace Polls.Lib.Database.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public Role Role { get; set; }
    }
}
