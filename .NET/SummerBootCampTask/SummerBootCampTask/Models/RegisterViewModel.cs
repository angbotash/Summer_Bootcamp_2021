using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Invalid email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is not defined.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Invalid password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Make my profile private")]
        public bool PrivateProfile { get; set; }
    }
}
