using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<string>();
            Claims = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        [Remote(action: "IsEmailInUse", controller: "account", AdditionalFields = "Id")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [ValidEmailDomain(allowedDomain: "pragimtech.com",
            ErrorMessage = "Email domain must be pragimtech.com")]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        public string City { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}
