using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheBlog_API.Auth;
using TheBlog_API.Data.Models;
using TheBlog_API.Results;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<AuthResult> RegisterUser(RegisterUserDto registerUserDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);

            if (existingUser != null)
            {
                return AuthResult.Fail("User with this email already exist.");
            }

            var user = new ApplicationUser
            {
                UserName = registerUserDto.UserName,
                Email = registerUserDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, BlogRoles.BlogUser);
                return AuthResult.Success(user);
            }

            return AuthResult.Fail("Minimum 8 characters, including at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        public async Task<AuthResult> Login(LoginUserDto user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);

            if (identityUser == null)
            {
                return AuthResult.Fail("Email not found.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, user.Password);

            if (isPasswordValid)
            {
                var roles = await _userManager.GetRolesAsync(identityUser);

                var accessToken = GenerateTokenString(identityUser.Email, identityUser.Id, roles);

                return AuthResult.SuccessLogin(accessToken);
            }

            return AuthResult.Fail("Invalid password.");
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);

            if (identityUser != null)
            {
                return identityUser;
            }

            return null;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);

            if (identityUser != null)
            {
                return identityUser;
            }

            return null;
        }

        public async Task<bool> ChangeWritePermissionsAsync(string userId, bool value)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);

            if (identityUser != null)
            {
                identityUser.CanWriteArticles = value;

                await _userManager.UpdateAsync(identityUser);

                return true;
            }

            return false;
        }

        public async Task<bool> ChangeRankPermissionsAsync(string userId, bool value)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);

            if (identityUser != null)
            {
                identityUser.CanRankArticles = value;

                await _userManager.UpdateAsync(identityUser);

                return true;
            }

            return false;
        }

        public async Task<bool> ChangeCommentingPermissionsAsync(string userId, bool value)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);

            if (identityUser != null)
            {
                identityUser.CanWriteComments = value;

                await _userManager.UpdateAsync(identityUser);

                return true;
            }

            return false;
        }

        public async Task<string> GetPasswordResetToken(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return token;
        }



        public async Task<IdentityResult> ResetPasswordAsync(string email, string password, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);

                if(resetPasswordResult.Succeeded)
                {
                    return IdentityResult.Success;
                }
            }

            var errors = new List<IdentityError>
            {
                new IdentityError { Code = "UserNotFound", Description = "User not found." }
            };

            return IdentityResult.Failed(errors.ToArray());
            
        }

        public string GenerateTokenString(string email, string userId, IEnumerable<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
            };

            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));

            SigningCredentials signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Audience").Value,
                signingCredentials: signingCred );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
