using CompanyApp.Models;
using Microsoft.AspNetCore.Identity;

namespace CompanyApp.Data
{
    public static class ApplicationInitializer
    {
        public static async Task Initialize(HttpContext context)
        {
            UserManager<User> userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
            string adminEmail = "admin@gmail.com";
            string adminName = "admin@gmail.com";

            string password = "Asd_123";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new()
                {
                    Email = adminEmail,
                    UserName = adminName
                };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
