using Microsoft.AspNetCore.Identity;
using TheBlog_API.Data.Models;

namespace TheBlog_API.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<IdentityResult> ResetPasswordAsync(string email, string password, string oldPassword);
        Task<bool> UpdateProfileInfo(string email, string firstName, string lastName, string bio);
    }
}