﻿using System;
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
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string phoneNumber { get; set; }
        public string ReturnUrl { get; set; }
        
    }
}