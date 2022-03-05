using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }
    }
}
