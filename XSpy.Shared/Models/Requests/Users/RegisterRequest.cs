using System.ComponentModel.DataAnnotations;

namespace XSpy.Database.Models.Requests.Users
{
    public class RegisterRequest
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Fullname { get; set; }
        [Required] public string Password { get; set; }
    }
}