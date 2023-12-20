using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBlog_API.Auth;
using TheBlog_API.Data.Models;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICommentService _commentService;

        public AdminController(IAuthService authService, ICommentService commentService)
        {
            _authService = authService;
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> ChangePermmisionToWrite(PermissionDto user)
        {
            if(!User.IsInRole(BlogRoles.Admin))
            {
                return Forbid();
            }

            var result = await _authService.ChangeWritePermissionsAsync(user.Id, user.Value);

            if(result)
            {
                return Ok("Writing permission succesfully granted.");
            }

            return BadRequest("Could not grant permissions.");
        }

        [HttpPost("Rank")]
        public async Task<IActionResult> ChangePermmisionToRank(PermissionDto user)
        {
            if (!User.IsInRole(BlogRoles.Admin))
            {
                return Forbid();
            }

            var result = await _authService.ChangeRankPermissionsAsync(user.Id, user.Value);

            if (result)
            {
                return Ok("Ranking permission succesfully granted.");
            }

            return BadRequest("Could not grant permissions.");
        }

        [HttpPost("Comment")]
        public async Task<IActionResult> ChangePermmisionToComment(PermissionDto user)
        {
            if (!User.IsInRole(BlogRoles.Admin))
            {
                return Forbid();
            }

            var result = await _authService.ChangeCommentingPermissionsAsync(user.Id, user.Value);

            if (result)
            {
                return Ok("Commenting Articles permission succesfully granted.");
            }

            return BadRequest("Could not grant permissions.");
        }

        [HttpGet("Reports")]
        public async Task<IActionResult> GetAllReports()
        {
            var result = await _commentService.GetAllReportedComments(User);
            return result;
        }
    }
}
