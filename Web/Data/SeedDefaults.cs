using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMinimal.Data;

namespace Web.Data
{
    public static class SeedDefaults
    {
        public enum IdentityRoleEnum
        {
            Admin,
            PowerUser,
            User
        }

        public enum IdentityUserEnum
        {
            Admin
        }

        public static object InitialStructure = new
        {
            Users = new { Admin = "Initial1!", PowerUser = "Initial1!", User = "Initial1!" },
            Roles = new { Admin = "Admin", PowerUser = "Admin, PowerUser", User = "Admin, PowerUser, User" },
            Pages = new
            {
                Accounts = new
                {
                    Admin = "*",
                    PowerUser = "r"
                },
                Roles = new
                {
                    Admin = "*",
                    PowerUser = "r"
                },
                Dashboard = new
                {
                    Admin = "*",
                    PowerUser = "*",
                    User = "r"
                },
                PageNormal = new
                {
                    Admin = "*",
                    PowerUser = "*",
                    User = "*"
                },
                PagePower = new
                {
                    Admin = "*",
                    PowerUser = "*",
                    User = "r"
                },
                PageAdmin = new
                {
                    Admin = "*",
                    PowerUser = "r"
                }
            }
        };

        public const string InitialPassword = "Initial1!";
        static Dictionary<IdentityUserEnum, IdentityRoleEnum[]> RolesMap = new Dictionary<IdentityUserEnum, IdentityRoleEnum[]>
        {
            { IdentityUserEnum.Admin, new IdentityRoleEnum[] { IdentityRoleEnum.User, IdentityRoleEnum.PowerUser, IdentityRoleEnum.Admin } }
        };

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            await AddDefaultRoles(serviceProvider);
            await AddDefaultUsers(serviceProvider);
        }

        private static async Task AddDefaultUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            foreach (var userName in Enum.GetNames(typeof(IdentityUserEnum)))
            {
                var user = await userManager.FindByEmailAsync(userName + "Demo@gmail.com");
                if (user == null)
                {
                    var un = userName + Guid.NewGuid().ToString();
                    user = new ApplicationUser()
                    {
                        UserName = un,
                        Email = userName + "Demo@gmail.com",
                        EmailConfirmed = true,
                        FirstName = un,
                        LastName = "DEMO",
                        Level = 1,
                        JoinDate = DateTime.Now.AddYears(-3)
                    };
                    var res = await userManager.CreateAsync(user, InitialPassword);
                    RolesMap
                        .Where(map => map.Key.ToString() == userName)
                        .ToList()
                        .ForEach(async (fe) =>
                            await userManager.AddToRolesAsync(user, fe.Value.Select(s => s.ToString())));
                }
            }
        }
        private static async Task AddDefaultRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Enum.GetNames(typeof(IdentityRoleEnum)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await add_claims(roleManager, new string[] { "acc", "role", "dashbrd" }, "Admin", "*");
            await add_claims(roleManager, new string[] { "acc", "role", "dashbrd" }, "PowerUser", "r");
            await add_claims(roleManager, new string[] { "dashbrd" }, "User", "r");
        }

        private static async Task add_claims(
            RoleManager<IdentityRole> roleManager,
            string[] Claims,
            string roleName,
            string claimValue)
        {
            var adminRole = await roleManager.FindByNameAsync(roleName);
            var claims = await roleManager.GetClaimsAsync(adminRole);
            Claims.Where(w => !claims.Any(a => a.Type == w))
                .ToList()
                .ForEach((fe) => roleManager.AddClaimAsync(adminRole, new Claim(fe, claimValue)).Wait());
            claims.Where(w => !Claims.Any(a => a == w.Type))
                .ToList()
                .ForEach((fe) => roleManager.RemoveClaimAsync(adminRole, fe).Wait());
        }
    }
}
