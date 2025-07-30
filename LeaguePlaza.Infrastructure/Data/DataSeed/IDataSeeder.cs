namespace LeaguePlaza.Infrastructure.Data.DataSeed
{
    public interface IDataSeeder
    {
        Task EnsureRoleSeedAsync();

        Task EnsureDefaultLeagueMasterSeedAsync();

        Task SeedTestQuestGiversAsync();

        Task SeedTestDataAsync();
    }
}
