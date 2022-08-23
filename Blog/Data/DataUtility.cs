using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Blog.Data
{
    public static class DataUtility
    {
        private const string _adminEmail = "seanbowers14@gmail.com";
        private const string _modEmail = "theseanbowers@gmail.com";
        private const string _adminRole = "Administrator";
        private const string _modRole = "Moderator";

        //Format dates into
        public static DateTime GetPostGresDate(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        //Connection string, local or Heroku
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        //If Heroku, use this builder
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }
        public static async Task SeedDataAsync(IServiceProvider svcProvider)
        {
            //Service: An instance of RoleManager.
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Migration: This is the programmatic equivalent to update-database.
            await dbContextSvc.Database.MigrateAsync();

            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

            //Serivce: An instance of RoleManager.
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Serivce: An instance of UserManager Service.
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            //Seed Roles
            await SeedRolesAsync(roleManagerSvc);
            //Seed Users
            await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);

        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //If no _adminRole create one.
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }
            //If no _modeRole create one.
            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));
            }
        }
        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Blog",
                    LastName = "Admin",
                    PhoneNumber = "7574038968",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, configuration["AdminPwd"] ?? Environment.GetEnvironmentVariable("AdminPwd"));
                await userManager.AddToRoleAsync(adminUser, _adminRole);
            }
            if (!context.Users.Any(u => u.Email == _modEmail))
            {
                BlogUser modUser = new()
                {
                    Email = _modEmail,
                    UserName = _modEmail,
                    FirstName = "Blog",
                    LastName = "Moderator",
                    PhoneNumber = "7574038968",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(modUser, configuration["ModeratorPwd"] ?? Environment.GetEnvironmentVariable("ModeratorPwd"));
                await userManager.AddToRoleAsync(modUser, _modRole); 
            }
        }
    }
}
