using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_contacts")]

    public class Contact : BaseDeviceEntity
    {
        [Column("number")]public string Number { get; set; }
        [Column("contact_name")]public string ContactName { get; set; }
    }
}