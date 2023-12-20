using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TheBlog_API.Auth;
using TheBlog_API.Data.Models;
using TheBlog_API.Email.Models;
using TheBlog_API.Email.Services;
using TheBlog_API.Results;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public AuthController(IAuthService authService, IEmailService emailService )
        {
            _authService = authService;
            _emailService = emailService;
        }

        [Authorize(Roles = BlogRoles.Admin)]
        [HttpGet]
        [Route("Users")]
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await _authService.GetAllAsync();

            return users.Select(u => new UserDto(u.Id, u.Email, u.UserName, u.CanWriteArticles, u.CanRankArticles, u.CanWriteComments));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto user)
        {
            AuthResult loginResult = await _authService.RegisterUser(user);

            if (loginResult.IsSuccess)
            {
                return Ok("Succesfully registered");
            }

            return BadRequest(loginResult.ErrorMessage);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDto user)
        {
            AuthResult loginResult = await _authService.Login(user);

            if (loginResult.IsSuccess)
            {
                return Ok(loginResult.Token);
            }
            return BadRequest(loginResult.ErrorMessage);
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _authService.GetUserByEmail(email);

            if(user != null)
            {
                var token = await _authService.GetPasswordResetToken(user);
                var forgotPasswordLink = Url.Action(
                    action: nameof(RestorePassword),
                    controller: "Auth",
                    values: new { token, email = user.Email },
                    protocol: "http",
                    host: "localhost:3000",
                    fragment: null
);
                var message = new Message(new string[] {user.Email}, "Forgot password link", forgotPasswordLink!);
                _emailService.SendEmail(message);
                return Ok($"Password change request is sent to {user.Email}. Please open your email and click the link.");
            }
            return BadRequest("Could not send email.");
        }

        [HttpGet]
        public async Task<IActionResult> RestorePassword(string token, string email)
        {
            var model = new RestorePasswordDto { Email = email, Token = token };

            return Ok(new 
            { 
                model 
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RestorePassword")]
        public async Task<IActionResult> RestorePassword(RestorePasswordDto restorePassword)
        {
            var resetPassResult = await _authService.ResetPasswordAsync(restorePassword.Email, restorePassword.Password, restorePassword.Token);
            if (resetPassResult.Succeeded)
            {
                return Ok($"Password changed successfully for {restorePassword.Email}. Please log in with your new password.");
            }
            return BadRequest();
        }

        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Name);

            if (userEmailClaim != null)
            {
                var userEmail = userEmailClaim.Value;

                ApplicationUser currentUser = await _authService.GetUserByEmail(userEmail);

                if (currentUser != null)
                {
                    return Ok(currentUser);
                }
            }

            return BadRequest("Unable to retrieve current user.");
        }
    }
}
