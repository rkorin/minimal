using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMinimal.Data;

namespace Web.Data
{
    public static class SeedDefaults
    {
        private const string InitialPassword = "Initial1!";
        private const string InitialEmail = "$loginDemo@gmail.com";
        private const string AdminUser = "Admin";

        private static object InitialStructure = new
        {
            Users = new { Admin = InitialPassword, PowerUser = InitialPassword, User = InitialPassword },
            Roles = new { Admin = "'Admin'", PowerUser = "'Admin', 'PowerUser'", User = "'Admin', 'PowerUser', 'User'" },
            Pages = new
            {
                Accounts = new
                {
                    Admin = "full",
                    PowerUser = "ro"
                },
                Roles = new
                {
                    Admin = "full",
                    PowerUser = "ro"
                },
                Dashboard = new
                {
                    Admin = "full",
                    PowerUser = "full",
                    User = "ro"
                },
                PageNormal = new
                {
                    Admin = "full",
                    PowerUser = "full",
                    User = "full"
                },
                PagePower = new
                {
                    Admin = "full",
                    PowerUser = "full",
                    User = "ro"
                },
                PageAdmin = new
                {
                    Admin = "full",
                    PowerUser = "ro"
                }
            }
        };

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var AdminEmail = InitialEmail.Replace("$login", AdminUser);
            var user = await userManager.FindByEmailAsync(AdminEmail);
            if (user != null) return;

            await AddDefaultRoles(serviceProvider, InitialStructure);
            await AddDefaultUsers(serviceProvider, InitialStructure);
        }

        private static async Task AddDefaultUsers(IServiceProvider serviceProvider, object Inititial)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var pusers_obj = pget(Inititial, "Users");
            foreach (var UserName in pgets(pusers_obj))
            {
                var user = new ApplicationUser()
                {
                    UserName = UserName,
                    Email = InitialEmail.Replace("$login", UserName),
                    EmailConfirmed = true,
                    FirstName = UserName,
                    LastName = "DEMO",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                };
                var res = await userManager.CreateAsync(user, InitialPassword);

                var roles = pget(Inititial, "Roles");
                List<Claim> claims = new List<Claim>();
                foreach (var RoleName in pgets(roles))
                {
                    var role_value = pget(roles, RoleName)?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(role_value))
                        continue;
                    if (role_value.Contains("'" + UserName + "'"))
                    {
                        userManager.AddToRoleAsync(user, RoleName).Wait();

                        var role = await roleManager.FindByNameAsync(RoleName);
                        if (role != null)
                        {
                            merge(claims, await roleManager.GetClaimsAsync(role));
                        }
                    }
                }
                if (claims.Count > 0)
                    userManager.AddClaimsAsync(user, claims).Wait();
            }

            /*   var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
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
               }*/
        }

        private static void merge(List<Claim> claims, IList<Claim> list)
        {
            foreach (var new_claim in list)
            {
                var first = claims.FirstOrDefault(f => f.Type == new_claim.Type);
                if (first == null)
                {
                    claims.Add(new_claim);
                }
                else
                {
                    if (new_claim.Value == "full")
                    {
                        claims.Remove(first);
                        claims.Add(new_claim);
                    }
                }
            }
        }

        private static async Task AddDefaultRoles(IServiceProvider serviceProvider, object Inititial)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var proles_obj = pget(Inititial, "Roles");
            var ppages_obj = pget(Inititial, "Pages");
            foreach (var RoleName in pgets(proles_obj))
            {
                var irole = new IdentityRole(RoleName);
                await roleManager.CreateAsync(irole);
                foreach (var PageName in pgets(ppages_obj))
                {
                    var ppage_obj = pget(ppages_obj, PageName);
                    string claim_value = pget(ppage_obj, RoleName)?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(claim_value))
                        continue;
                    await roleManager.AddClaimAsync(irole, new Claim(PageName, claim_value));
                }
            }
        }

        private static object pget(object o, string n)
        {
            return o?.GetType()?.GetProperty(n)?.GetValue(o);
        }

        private static List<string> pgets(object o)
        {
            return o?.GetType()?.GetProperties()?.Select(s => s.Name)?.ToList() ?? new List<string>();
        }
    }
}
