using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.DTO
{
    public class CreateUserDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]+([ a-zA-Z0-9_.]{0,19})*$", ErrorMessage = "Invalid Username")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#?!@$%^&*-])[A-Za-z\d#?!@$%^&*-]{8,}$", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
    }

    public class UserLoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }
    }

    public class TokenDto
    {
        public string Token { get; set; }
    }
}
