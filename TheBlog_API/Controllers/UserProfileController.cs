using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheBlog_API.Auth;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Controllers
{
    [Route("api/Profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto userData)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Name);

            if (userEmailClaim != null)
            {
                var userEmail = userEmailClaim.Value;

                var result = await _userProfileService.ResetPasswordAsync(userEmail, userData.Password, userData.OldPassword);

                if (result.Succeeded)
                {
                    return Ok("Password reset successful");
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Unable to retrieve current user.");
        }

        [HttpPost("ChangeProfile")]
        [Authorize]
        public async Task<IActionResult> ChangeProfile(UpdateProfileDto info)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Name);

            if (userEmailClaim != null)
            {
                var userEmail = userEmailClaim.Value;

                var result = await _userProfileService.UpdateProfileInfo(userEmail, info.FirstName, info.LastName, info.Bio);

                if (result)
                {
                    return Ok("Changes applied to user profile.");
                }

                return BadRequest(result);
            }

            return BadRequest("Unable to retrieve current user.");
        }
    }
}
