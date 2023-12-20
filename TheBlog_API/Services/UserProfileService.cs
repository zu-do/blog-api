using Microsoft.AspNetCore.Identity;
using TheBlog_API.Data.Models;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string password, string oldPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var errors = new List<IdentityError>
                {
                    new IdentityError { Code = "UserNotFound", Description = "User not found." }
                };

                return IdentityResult.Failed(errors.ToArray());
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, oldPassword);

            if (!isPasswordValid)
            {
                var errors = new List<IdentityError>
                {
                    new IdentityError { Code = "InvalidOldPassword", Description = "The old password is incorrect." }
                };

                return IdentityResult.Failed(errors.ToArray());
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<bool> UpdateProfileInfo(string email, string firstName, string lastName, string bio)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Bio = bio;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
