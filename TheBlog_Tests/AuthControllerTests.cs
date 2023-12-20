using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using TheBlog_API.Auth;
using TheBlog_API.Controllers;
using TheBlog_API.Data.Models;
using TheBlog_API.Email.Services;
using TheBlog_API.Results;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly AuthController _authController;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        public AuthControllerTests() 
        {
            _authServiceMock = new Mock<IAuthService>();
            _emailServiceMock = new Mock<IEmailService>();
            _urlHelperMock = new Mock<IUrlHelper>();

            var existingUser = new ApplicationUser { Email = "existing@example.com" };

            _authServiceMock.Setup(x => x.RegisterUser(It.IsAny<RegisterUserDto>()))
                .ReturnsAsync((RegisterUserDto _) => AuthResult.Success(new ApplicationUser()));
            _authServiceMock.Setup(x => x.Login(It.IsAny<LoginUserDto>()))
                .ReturnsAsync(AuthResult.Success(new ApplicationUser()));
            _authServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(existingUser);
            _authServiceMock.Setup(x => x.GetPasswordResetToken(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("mocked_reset_token");
            _urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost:3000/restorepassword?token=mocked_reset_token&email=existing%40example.com");

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            _authController = new AuthController(_authServiceMock.Object, _emailServiceMock.Object)
            {
                ControllerContext = controllerContext,
            };
        }

        [Fact]
        public async Task RegisterUser_ValidUser_ReturnsOk()
        {
            var result = await _authController.
                RegisterUser(new RegisterUserDto("test@gmail.com", "test", "Valid1@"));

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidUser_ReturnsOk()
        {
            var result = await _authController.Login(new LoginUserDto("test@gmail.com", "Valid1@"));

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_ExistingUser_ReturnsOk()
        {
            _authController.Url = _urlHelperMock.Object;

            var result = await _authController.ForgotPassword("existing@example.com");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RestorePassword_ValidTokenAndEmail_ReturnsOk()
        {
            var result = await _authController.RestorePassword("mocked_token", "mocked_email");

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            Assert.NotNull(value);
            var modelProperty = value.GetType().GetProperty("model");

            Assert.NotNull(modelProperty);
            var modelValue = modelProperty.GetValue(value);

            var restorePasswordDto = Assert.IsType<RestorePasswordDto>(modelValue);
            Assert.Equal("mocked_email", restorePasswordDto.Email);
            Assert.Equal("mocked_token", restorePasswordDto.Token);
        }
    }
}
