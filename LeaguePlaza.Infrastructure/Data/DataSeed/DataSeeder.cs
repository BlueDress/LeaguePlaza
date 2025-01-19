using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Infrastructure.Data.DataSeed
{
    public class DataSeeder(RoleManager<IdentityRole> roleManager) : IDataSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task EnsureRoleSeedAsync()
        {
            string[] roleNames = ["League Master", "Adventurer", "Quest Giver"];

            foreach (var roleName in roleNames)
            {
                bool roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
