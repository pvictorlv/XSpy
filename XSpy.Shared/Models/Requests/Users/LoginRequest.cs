﻿using System.ComponentModel.DataAnnotations;

namespace XSpy.Database.Models.Requests.Users
{
    public class LoginRequest
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}