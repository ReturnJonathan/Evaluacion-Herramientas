﻿using System.ComponentModel.DataAnnotations;

namespace DesafioH.App.Models.Auth
{
    public class LoginVM
    {

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }

    }
}
