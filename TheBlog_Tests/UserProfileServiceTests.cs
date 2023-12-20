using Microsoft.AspNetCore.Identity;
using Moq;
using TheBlog_API.Data.Models;
using TheBlog_API.Services;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_Tests
{
    public class UserProfileServiceTests
    {
        private readonly IUserProfileService _userProfileService;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        public UserProfileServiceTests()
        {
            _userManager = MockUserManager(_users);
            _userProfileService = new UserProfileService(_userManager.Object);
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : ApplicationUser
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) => ls.FirstOrDefault(u => u.Email == email));

            return mgr;
        }

        private List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser() { Id = "1", UserName = "User1", Email = "user1@bv.com" },
            new ApplicationUser() { Id = "2", UserName = "User2", Email = "user2@bv.com" }
        };

        [Fact]
        public async Task UpdateProfileInfo_ChangesExistingUserInfo()
        {
            var userEmail = "user1@bv.com";

            var result  = await _userProfileService.UpdateProfileInfo(userEmail, "TestName", "TestSurname", "TestBio");

            Assert.Equal("TestBio", _users[0].Bio);
            Assert.Equal("TestName", _users[0].FirstName);
            Assert.Equal("TestSurname", _users[0].LastName);
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateProfileInfo_WithNonExistUser_ReturnFalse()
        {
            var userEmail = "user111@bv.com";

            var result = await _userProfileService.UpdateProfileInfo(userEmail, "TestName", "TestSurname", "TestBio");

            Assert.False(result);
        }

        [Fact]
        public async Task ResetPassword_WithNonExistUser_ReturnUserNotFoundError()
        {
            var userEmail = "user111@bv.com";

            var result = await _userProfileService.ResetPasswordAsync(userEmail, "TestName", "TestSurname");

            var expectedError = Assert.Single(result.Errors);
            Assert.Equal("UserNotFound", expectedError.Code);
            Assert.Equal("User not found.", expectedError.Description);
        }

        [Fact]
        public async Task ResetPassword_WithNotValidPassword_ReturnInvalidPasswordError()
        {
            var userEmail = "user1@bv.com";
            _userManager.Setup(x => x.CheckPasswordAsync(_users[0], "invalidPassword"))
                .ReturnsAsync(false);
            var result = await _userProfileService.ResetPasswordAsync(userEmail, "TestName", "invalidPassword");

            var expectedError = Assert.Single(result.Errors);
            Assert.Equal("InvalidOldPassword", expectedError.Code);
            Assert.Equal("The old password is incorrect.", expectedError.Description);
        }
    }
}
