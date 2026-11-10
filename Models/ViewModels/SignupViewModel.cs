using System.ComponentModel.DataAnnotations;

namespace LedgerPro.Models.ViewModels
{
    public class SignupViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please select an account type")]
        [Display(Name = "Account Type")]
        public string Role { get; set; } = "Staff"; 

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
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