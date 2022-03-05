using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name ="Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
