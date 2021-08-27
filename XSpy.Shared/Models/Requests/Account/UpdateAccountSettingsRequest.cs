using System;
using System.ComponentModel.DataAnnotations;

namespace XSpy.Database.Models.Requests.Account
{
    public class UpdateAccountSettingsRequest
    {
        public string Name { get; set; }
        [EmailAddress] public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public string Document { get; set; }
        public string ZipCode { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}