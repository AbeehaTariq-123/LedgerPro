using Microsoft.AspNetCore.Identity;

namespace LedgerPro.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            foreach (var role in new[] { "Admin", "Staff" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var email = config["SeedAdmin:Email"] ?? "admin@ledgerpro.com";
            var password = config["SeedAdmin:Password"] ?? "Admin@12345";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}