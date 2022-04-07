using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Intex2022.Models.ViewModels
{
    public class RegistrationModel
    {
       
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string phoneNumber { get; set; }
        public string ReturnUrl { get; set; }
        
    }
}
