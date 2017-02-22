using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMinimal.Data;
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
               //|| !context.Request.Body.Length>0
               )
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            string body = "";
            using (var bodyReader = new StreamReader(context.Request.Body))
            {
                body = bodyReader.ReadToEnd();
            }

            LoginViewModelRequest req = JsonConvert.DeserializeObject<LoginViewModelRequest>(body);
            if (body != null)
                return GenerateToken(context, req);

            return _next(context);
        }

        public async Task<LoginViewModelResponse> Login(string Email, string Password)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, Password, true,
               lockoutOnFailure: false);
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
            }

            return new LoginViewModelResponse { Success = false, message = "Invalid login attempt." };

        }

        private async Task GenerateToken(HttpContext context,
            LoginViewModelRequest req)
        {

            var username = req.Email;
            var password = req.Password;

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
                new Claim(JwtRegisteredClaimNames.Iat, dto.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Email, result_user.User.Email, ClaimValueTypes.Integer64)
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
