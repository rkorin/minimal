using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMinimal.Models.AccountViewModels
{
    public class LoginViewModelRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class LoginViewModelResponse
    {
        internal string message;

        public bool Success { get; set; }
        public bool NeedTwoFactor { get; set; }
        public string Token { get; set; }
        public bool IsLocked { get; internal set; }
        public ApplicationUser User { get; internal set; }
    }
}
