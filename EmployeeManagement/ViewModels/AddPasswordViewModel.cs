using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
