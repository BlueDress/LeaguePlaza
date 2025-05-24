using LeaguePlaza.Common.Constants;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Infrastructure.Data.DataSeed
{
    public class DataSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : IDataSeeder
    {
        private readonly Dictionary<string, string> DefaultQuestTypeImages = new()
        {
            { "MonsterHunt", "https://www.dropbox.com/scl/fi/zxqv1fy2io88ytcdi3iqa/monster-hunt-default.jpg?rlkey=vkl9dt9q96af2qlv8gx5etsdy&st=03rctf0o&raw=1" },
            { "Gathering", "https://www.dropbox.com/scl/fi/ns7u5n9zhqw9q3i5g6gsq/gathering-default.jpg?rlkey=zbrno8iqnhxdqgmm2xkg8moyh&st=gm6ja4j6&raw=1" },
            { "Escort", "https://www.dropbox.com/scl/fi/977mmg7o6fxpr3e4i5k4p/escort-default.jpg?rlkey=fyekeazwrh373cyxqtu6kjxeg&st=2y5oj0ms&raw=1" },
        };

        private readonly Dictionary<string, string> DefaultMountTypeImages = new()
        {
            { "Ground", "https://www.dropbox.com/scl/fi/wyvpahi0salv5ii2v5i8r/ground-default.jpg?rlkey=br72tc41gyn9b59bqk5ahyyod&st=w147fofs&raw=1" },
            { "Flying", "https://www.dropbox.com/scl/fi/9n7d7geaprae40gjhcvyr/flying-default.jpg?rlkey=ktap2t7jjgo2j8oatka34rxdu&st=pfuagymz&raw=1" },
            { "Aquatic", "https://www.dropbox.com/scl/fi/soux4avtf2hlpjw2gguth/aquatic-default.jpg?rlkey=hlf1n9g4pts8zrcfiaiglddyu&st=om0epfjz&raw=1" },
        };

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
            var adventurers = _applicationDbContext.UserRoles.Where(ur => ur.RoleId == _roleManager.Roles.First(r => r.Name == UserRoleConstants.Adventurer).Id).ToArray();

            if (!_applicationDbContext.Quests.Any())
            {
                var testQuests = new List<QuestEntity>();
                var questGivers = _applicationDbContext.UserRoles.Where(ur => ur.RoleId == _roleManager.Roles.First(r => r.Name == UserRoleConstants.QuestGiver).Id).ToArray();

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
                        AdventurerId = i % 9 == 0 ? adventurers[i % 2].UserId : null,
                        ImageName = i % 3 == 0 ? DefaultQuestTypeImages["MonsterHunt"] : i % 3 == 1 ? DefaultQuestTypeImages["Escort"] : DefaultQuestTypeImages["Gathering"],
                    };

                    testQuests.Add(newTestQuest);
                }

                await _applicationDbContext.Quests.AddRangeAsync(testQuests);
                await _applicationDbContext.SaveChangesAsync();
            }

            if (!_applicationDbContext.Mounts.Any())
            {
                var testMounts = new List<MountEntity>();

                for (int i = 1; i <= 30; i++)
                {
                    var newTestMount = new MountEntity
                    {
                        Name = $"Test Mount {i}",
                        Description = i % 5 == 0 ? string.Empty : "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                        RentPrice = Math.Round((i + 5) / 17m, 2) * 100,
                        ImageUrl = i % 3 == 0 ? DefaultMountTypeImages["Ground"] : i % 3 == 1 ? DefaultMountTypeImages["Flying"] : DefaultMountTypeImages["Aquatic"],
                        MountType = i % 3 == 0 ? MountType.Ground : i % 3 == 1 ? MountType.Flying : MountType.Aquatic,
                    };

                    if (i % 3 == 0)
                    {
                        for (int j = 0; j < adventurers.Length; j++)
                        {
                            newTestMount.MountRatings.Add(new MountRatingEntity()
                            {
                                Rating = (i + j) % 6,
                                UserId = adventurers[j].UserId,
                            });

                            newTestMount.MountRentals.Add(new MountRentalEntity()
                            {
                                StartDate = DateTime.UtcNow.AddMonths(j).AddDays(i),
                                EndDate = DateTime.UtcNow.AddMonths(j).AddDays(i + j),
                                UserId = adventurers[j].UserId,
                            });
                        }
                    }
                    else if (i % 3 == 1)
                    {
                        newTestMount.MountRatings.Add(new MountRatingEntity()
                        {
                            Rating = i % 6,
                            UserId = adventurers[i % 2].UserId,
                        });

                        newTestMount.MountRentals.Add(new MountRentalEntity()
                        {
                            StartDate = DateTime.UtcNow.AddMonths(i % 2).AddDays(i),
                            EndDate = DateTime.UtcNow.AddMonths(i % 2).AddDays(i + i % 2),
                            UserId = adventurers[i % 2].UserId,
                        });
                    }

                    testMounts.Add(newTestMount);
                }

                await _applicationDbContext.Mounts.AddRangeAsync(testMounts);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
