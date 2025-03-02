using LeaguePlaza.Common.Constants;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Infrastructure.Data.DataSeed
{
    public class DataSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : IDataSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task EnsureRoleSeedAsync()
        {
            string[] roleNames = [UserRoleConstants.LeagueMaster, UserRoleConstants.Adventurer, UserRoleConstants.QuestGiver];

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
                await _userManager.AddToRoleAsync(defaultLeagueMaster, UserRoleConstants.LeagueMaster);
            }
        }

        public async Task SeedTestDataAsync()
        {
            if (!_applicationDbContext.Quests.Any())
            {
                var testQuests = new List<QuestEntity>();
                var questGivers = _applicationDbContext.UserRoles.Where(ur => ur.RoleId == _roleManager.Roles.First(r => r.Name == UserRoleConstants.QuestGiver).Id).ToArray();
                var adventurers = _applicationDbContext.UserRoles.Where(ur => ur.RoleId == _roleManager.Roles.First(r => r.Name == UserRoleConstants.Adventurer).Id).ToArray();

                for (int i = 1; i <= 100; i++)
                {
                    var newTestQuest = new QuestEntity
                    {
                        Title = $"Test Quest {i}",
                        Description = i % 7 == 0 ? string.Empty : "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                        Created = DateTime.Now.AddDays(-i),
                        RewardAmount = Math.Round((i + 7) / 17m, 2) * 100,
                        Type = i % 3 == 0 ? QuestType.MonsterHunt : i % 3 == 1 ? QuestType.Escort : QuestType.Gathering,
                        Status = i % 9 == 0 ? QuestStatus.Accepted : QuestStatus.Posted,
                        CreatorId = questGivers[i % 2].UserId,
                        AdventurerId = i % 9 == 0 ? adventurers[i % 2].UserId : null
                    };

                    testQuests.Add(newTestQuest);
                }

                await _applicationDbContext.Quests.AddRangeAsync(testQuests);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
