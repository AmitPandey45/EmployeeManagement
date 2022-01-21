﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [Remote(action: "IsEmailInUse", controller: "account")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
