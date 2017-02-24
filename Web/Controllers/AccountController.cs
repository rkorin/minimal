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
using Web.Models.Account;

namespace Web.Controllers
{

    [Route("api/[controller]")]
    [Authorize(Policy = "Accounts")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        public IEnumerable<AccountModel> Get()
        {
            //return _userManager.
            return new List<AccountModel>();
        }

    }
}
