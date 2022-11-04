using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Polls.Lib.Repositories.Authentication
{
    internal sealed class UserAuthenticationRepository : IUserAuthenticationRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserAuthenticationRepository(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        #region Public methods

        public async Task<IdentityResult> RegisterUserAsync(CreateUserDto userRegistration, Role role)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userRegistration.UserName,
                Email = userRegistration.Email,
                FullName = userRegistration.FullName,
                Address = userRegistration.Address,
                Role = role
            };

            var result = await _userManager.CreateAsync(user, userRegistration.Password);
            return result;
        }

        public async Task<UserValidationResult> ValidateUserAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                return new UserValidationResult()
                {
                    Authorized = false,
                    ValidationMessage = "User not exists."
                };
            }

            var result = user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (result == false)
            {
                return new UserValidationResult()
                {
                    Authorized = false,
                    ValidationMessage = "Password incorrect."
                };
            }

            string token = await CreateTokenAsync(user);

            return new UserValidationResult()
            {
                Authorized = true,
                Token = token
            };
        }
        public Task<User> GetUserByName(string username)
        {
            return _userManager.FindByNameAsync(username);
        }

        public Task<User> GetUserById(Guid userId)
        {
            return _userManager.FindByIdAsync(userId.ToString());
        }

        #endregion

        #region Private methods

        private async Task<string> CreateTokenAsync(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = _configuration.GetSection("JwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expiresIn"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }

        #endregion
    }
}
