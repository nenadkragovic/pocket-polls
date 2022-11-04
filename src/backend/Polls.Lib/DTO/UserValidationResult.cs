using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polls.Lib.DTO
{
    public class UserValidationResult
    {
        public bool Authorized { get; set; }
        public string Token { get; set; }
        public string ValidationMessage { get; set; }
    }
}
