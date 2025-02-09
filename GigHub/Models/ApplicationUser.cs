﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GigHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100), Display(Name = "Full name")]
        public string Name { get; set; }
    }
}
