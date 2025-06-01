using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Services;
using LeaguePlaza.Core.Features.Home.Contracts;
using LeaguePlaza.Core.Features.Home.Services;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Services;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Services;
using LeaguePlaza.Infrastructure.Data;
using LeaguePlaza.Infrastructure.Data.DataSeed;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using LeaguePlaza.Infrastructure.Dropbox.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LeaguePlaza.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                            .AddRoles<IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped(typeof(IRepository), typeof(Repository));
            builder.Services.AddScoped(typeof(IDataSeeder), typeof(DataSeeder));

            builder.Services.AddTransient(typeof(IAdminService), typeof(AdminService));
            builder.Services.AddTransient(typeof(IHomeService), typeof(HomeService));
            builder.Services.AddTransient(typeof(IQuestService), typeof(QuestService));
            builder.Services.AddTransient(typeof(IMountService), typeof(MountService));
            builder.Services.AddTransient(typeof(IDropboxService), typeof(DropboxService));

            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dataSeeder = scope.ServiceProvider.GetService<IDataSeeder>();
                await dataSeeder!.EnsureRoleSeedAsync();
                await dataSeeder!.EnsureDefaultLeagueMasterSeedAsync();
                await dataSeeder!.SeedTestDataAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
