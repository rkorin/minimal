using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using WebMinimal.Models;
using WebMinimal.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebMinimal.Data;

namespace Web.Controllers
{

    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public bool Success { get; private set; }

        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                return _userManager;
            }
        }

        //private readonly IEmailSender _emailSender;
        //private readonly ISmsSender _smsSender;
        //private readonly ILogger _logger;

        public AccountController(
          UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleManager,
          SignInManager<ApplicationUser> signInManager/*,
          IEmailSender emailSender,
          ISmsSender smsSender,
          ILoggerFactory loggerFactory*/)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            //_smsSender = smsSender;
            //_logger = loggerFactory.CreateLogger<AccountController>();

        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<LoginViewModelResponse> Login([FromBody] LoginViewModelRequest model)
        {
            try { await CheckSeed(); }
            catch (Exception ex) { }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return new LoginViewModelResponse { Success = true };
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
        private async Task CheckSeed()
        {
            if (await UserManager.FindByNameAsync("SuperPowerUser") == null)
            {
                var spu = new ApplicationUser()
                {
                    UserName = "SuperPowerUser",
                    Email = "rkorin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Roman",
                    LastName = "Korin",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                };
                var sa = new ApplicationUser()
                {
                    UserName = "SuperAdmin",
                    Email = "rkorin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Roman",
                    LastName = "Korin",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                }; var a = new ApplicationUser()
                {
                    UserName = "Admin",
                    Email = "rkorin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Roman",
                    LastName = "Korin",
                    Level = 1,
                    JoinDate = DateTime.Now.AddYears(-3)
                };
                await UserManager.CreateAsync(spu, "123");                
                await UserManager.CreateAsync(sa, "123");
                await UserManager.CreateAsync(a, "123");

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
    }
}
