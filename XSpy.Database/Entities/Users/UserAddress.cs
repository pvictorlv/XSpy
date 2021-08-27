using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XSpy.Database.Entities
{
    [Table("user_addresses")]
    public class UserAddress
    {
        [Column("id")] public Guid? Id { get; set; }

        [Column("zip")] public string Zip { get; set; }
        [Column("street")] public string Street { get; set; }
        [Column("city")] public string City { get; set; }
        [Column("number")]
        public string Number { get; set; }

        [Column("neighborhood")] public string Neighborhood { get; set; }
        [Column("complement")] public string Complement { get; set; }
        [Column("state")] public string State { get; set; }

        [Column("user_id"), ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}