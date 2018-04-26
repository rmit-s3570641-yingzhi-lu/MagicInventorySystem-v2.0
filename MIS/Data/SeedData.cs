using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MIS.Models;

namespace MIS.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { Constants.RetailRole, Constants.WholeSaleRole };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            await EnsureUserHasRole(userManager, "retail@example.com", Constants.RetailRole);
            await EnsureUserHasRole(userManager, "wholesale@example.com", Constants.WholeSaleRole);
            await EnsureUserHasRole(userManager, "s3570641@student.rmit.edu.au", Constants.WholeSaleRole);
        }

        private static async Task EnsureUserHasRole(
            UserManager<ApplicationUser> userManager, string userName, string role)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null && !await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
