using MahantInv.Infrastructure.Identity;
using MahantInv.Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.SeedScripts
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<MIIdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Meta.Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Meta.Roles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(Meta.Roles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(Meta.Roles.User));
            }
        }

        private static async Task SeedUsers(UserManager<MIIdentityUser> userManager)
        {
            if (userManager.Users.All(u => u.UserName != "system"))
            {
                var user = new MIIdentityUser
                {
                    UserName = "system",
                    Email = "admin@system.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "M@hant@1309");
                await userManager.AddToRoleAsync(user, Meta.Roles.Admin);
            }
        }
    }


}
