using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TheBlog_API.Auth;
using TheBlog_API.Controllers;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_Tests
{
    public class UserProfileControllerTests
    {
        private readonly Mock<IUserProfileService> _userProfileService;
        private readonly UserProfileController _userProfileController;
        public UserProfileControllerTests()
        {
            _userProfileService = new Mock<IUserProfileService>();
            _userProfileController = new UserProfileController(_userProfileService.Object);

            _userProfileService.Setup(x => x.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);
            _userProfileService.Setup(x => x.UpdateProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _userProfileController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, "testuser@example.com")
                }))
                }
            };
        }
        [Fact]
        public async Task ResetPassword_ValidUserData_ReturnsOk()
        {
            var userData = new ResetPasswordDto("new_password", "old_password");

            var result = await _userProfileController.ResetPassword(userData);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password reset successful", okResult.Value);
        }

        [Fact]
        public async Task ChangeProfile_ValidUpdateProfileDto_ReturnsOk()
        {
            var info = new UpdateProfileDto("NewFirstName", "NewLastName", "NewBio");

            var result = await _userProfileController.ChangeProfile(info);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Changes applied to user profile.", okResult.Value);
        }
    }
}
