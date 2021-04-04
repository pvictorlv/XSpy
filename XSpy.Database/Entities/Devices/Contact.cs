using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_contacts")]

    public class Contact : BaseDeviceEntity
    {
        [Column("number"), MaxLength(50)]public string Number { get; set; }
        [Column("contact_name"), MaxLength(120)] public string ContactName { get; set; }
        [Column("phone_id"), MaxLength(40)] public string PhoneId { get; set; }
        [Column("hash"), MaxLength(32)] public string Hash { get; set; }
    }
}