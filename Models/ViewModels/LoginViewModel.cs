
using System.ComponentModel.DataAnnotations;

namespace LedgerPro.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please select an account type")]
        [RegularExpression("^(Admin|Staff)$", ErrorMessage = "Select Admin or Staff")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; } = "Staff";

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
