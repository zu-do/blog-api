using Microsoft.AspNetCore.Identity;
using TheBlog_API.Auth;
using TheBlog_API.Data.Models;
using TheBlog_API.Results;

namespace TheBlog_API.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateTokenString(string email, string userId, IEnumerable<string> roles);
        Task<IReadOnlyList<ApplicationUser>> GetAllAsync();
        Task<string> GetPasswordResetToken(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<ApplicationUser> GetUserById(string userId);
        Task<bool> ChangeRankPermissionsAsync(string userId, bool value);
        Task<bool> ChangeWritePermissionsAsync(string userId, bool value);
        Task<AuthResult> Login(LoginUserDto user);
        Task<AuthResult> RegisterUser(RegisterUserDto user);
        Task<IdentityResult> ResetPasswordAsync(string email, string password, string token);
        Task<bool> ChangeCommentingPermissionsAsync(string userId, bool value);
    }
}