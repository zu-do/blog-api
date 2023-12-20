using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using TheBlog_API.Auth;
using TheBlog_API.Data.Models;
using TheBlog_API.Services;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IAuthService _authService;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        public AuthServiceTests()
        {
            _userManager = MockUserManager(_users);
            _configuration = new Mock<IConfiguration>();
            _authService = new AuthService(_userManager.Object, _configuration.Object);
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
        public async Task RegisterUser_AddsUser()
        {
            var newUser = new RegisterUserDto("user4@bv.com", "User4", "P@ssw0rd!");
            await _authService.RegisterUser(newUser);
            Assert.Equal(3, _users.Count);
        }

        [Fact]
        public async Task Login_WithNonExistenceEmail_ReturnsFail()
        {
            var loginUserDto = new LoginUserDto("nonexistent@example.com", "invalidpassword");

            var result = await _authService.Login(loginUserDto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Email not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsFail()
        {
            var loginUserDto = new LoginUserDto("user1@bv.com", "invalidpassword");

            var result = await _authService.Login(loginUserDto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid password.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetUserByEmail_ExistingUser_ReturnsUser()
        {
            var result = await _authService.GetUserByEmail("user1@bv.com");
            
            Assert.NotNull(result);
            Assert.Equal(_users[0], result);
        }

        [Fact]
        public async Task GetUserByEmail_ExistingUser_ReturnsNull()
        {
            var result = await _authService.GetUserByEmail("nonexistence@bv.com");

            Assert.Null(result);
        }

    }
}