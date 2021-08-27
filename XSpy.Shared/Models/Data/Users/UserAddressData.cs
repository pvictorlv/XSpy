using System;

namespace Stock.Shared.Models.Data
{
    public class UserAddressData
    {
        public Guid? Id { get; set; }

        public string Zip { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public string Complement { get; set; }
        public string State { get; set; }
        public UserData User { get; set; }
    }
}