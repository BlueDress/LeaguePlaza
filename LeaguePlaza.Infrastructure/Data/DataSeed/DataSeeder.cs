using LeaguePlaza.Infrastructure.Data.Entities;

using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Infrastructure.Data.DataSeed
{
    public class DataSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) : IDataSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task EnsureRoleSeedAsync()
        {
            string[] roleNames = ["League Master", "Adventurer", "Quest Giver"];

            foreach (var roleName in roleNames)
            {
                bool roleExist = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public async Task EnsureDefaultLeagueMasterSeedAsync()
        {
            var defaultLeagueMaster = new ApplicationUser
            {
                UserName = "LeagueMaster@leaguemaster.com",
                Email = "LeagueMaster@leaguemaster.com",
                EmailConfirmed = true,
            };

            if (await _userManager.FindByEmailAsync(defaultLeagueMaster.Email) == null)
            {
                await _userManager.CreateAsync(defaultLeagueMaster, "LeagueMaster@123");
                await _userManager.AddToRoleAsync(defaultLeagueMaster, "League Master");
            }
        }
    }
}
