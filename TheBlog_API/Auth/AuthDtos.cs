using System.ComponentModel.DataAnnotations;

namespace TheBlog_API.Auth
{
    public record UserDto(string Id, string Email, string UserName, bool CanWriteArticles, bool CanRankArticles, bool CanWriteComments);
    public record LoginUserDto(string Email, string Password);
    public record RegisterUserDto(string Email, string UserName, string Password);
    public record ResetPasswordDto(string Password, string OldPassword);
    public record UpdateProfileDto(string FirstName, string LastName, string Bio);
    public record PermissionDto(string Id, bool Value);

    public class RestorePasswordDto
    {
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The password and connfirmation password does not macth.")]
        public string ConfirmPassword { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
