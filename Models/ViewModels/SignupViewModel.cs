
using System.ComponentModel.DataAnnotations;

namespace LedgerPro.Models.ViewModels
{
    public class SignupViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be 2–100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please select an account type")]
        [RegularExpression("^(Admin|Staff)$",
            ErrorMessage = "Select Admin or Staff")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; } = "Staff";

        [DataType(DataType.Password)]
        [Display(Name = "Admin Registration Code")]
        public string? AdminRegistrationCode { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
