using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMinimal.Models;
using WebMinimal.Models.AccountViewModels;

namespace Web
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options,
            UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleManager,
          SignInManager<ApplicationUser> signInManager)
        {
            _next = next;
            _options = options.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
        }

        private async Task CheckSeed()
        {
            if (await _userManager.FindByEmailAsync("rkorinSuperPowerUser@gmail.com") == null)
            {
                var spu = new ApplicationUser()
                {
                    UserName = "SuperPowerUser",
                    Email = "rkorinSuperPowerUser@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Roman",
                    LastName = "Korin",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                };
                var sa = new ApplicationUser()
                {
                    UserName = "SuperAdmin",
                    Email = "rkorinSuperAdmin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Roman",
                    LastName = "Korin",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                }; var a = new ApplicationUser()
                {
                    UserName = "Admin",
                    Email = "rkorinAdmin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Roman",
                    LastName = "Korin",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                };
                await _userManager.CreateAsync(spu, "123");
                await _userManager.CreateAsync(sa, "123");
                await _userManager.CreateAsync(a, "123");

                if (_roleManager.Roles.Count() == 0)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
                    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperUser" });
                    await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
                }
                //await _userManager.AddToRolesAsync(a, new string[] { "Admin", "SuperUser", "User" });
                //await _userManager.AddToRolesAsync(spu, new string[] { "SuperUser", "User" });
                //await _userManager.AddToRolesAsync(sa, new string[] { "Admin", "SuperAdmin", "SuperUser", "User" });
            }
        }

        public async Task<LoginViewModelResponse> Login(string Email, string Password)
        {
            
            await CheckSeed();
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(Email, Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                ApplicationUser au = await _userManager.FindByEmailAsync(Email);
                return new LoginViewModelResponse { Success = false, User = au };
            }
            if (result.RequiresTwoFactor)
            {
                return new LoginViewModelResponse { Success = false, NeedTwoFactor = true };
            }
            if (result.IsLockedOut)
            {
                return new LoginViewModelResponse { Success = false, IsLocked = true };
            }
            else
            {
                return new LoginViewModelResponse { Success = false, message = "Invalid login attempt." };
            }
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["email"];
            var password = context.Request.Form["password"];

            var result_user = await Login(username, password);
            if (result_user == null || result_user.User == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;
            DateTimeOffset dto = new DateTimeOffset(now);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, dto.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
    }
}
