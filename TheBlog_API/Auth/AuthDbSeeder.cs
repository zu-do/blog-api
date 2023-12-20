using Microsoft.AspNetCore.Identity;
using TheBlog_API.Constants;
using TheBlog_API.Data.Models;

namespace TheBlog_API.Auth
{
    public class AuthDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthDbSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await AddDefaultRoles();
            await AddAdminUser();
        }

        private async Task AddAdminUser()
        {
            var newAdminUser = new ApplicationUser
            {
                UserName = AdminUserConstants.Username,
                Email = AdminUserConstants.Email,
            };

            var existingAdminUser = await _userManager.FindByEmailAsync(newAdminUser.Email);

            if (existingAdminUser == null)
            {
                var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, AdminUserConstants.Password);

                if(createAdminUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newAdminUser, BlogRoles.All);
                }
            }
        }

        public async Task AddDefaultRoles()
        {
            foreach (var role in BlogRoles.All)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
