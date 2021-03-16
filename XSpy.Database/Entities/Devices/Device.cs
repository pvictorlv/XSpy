using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XSpy.Database.Entities.Devices
{
    [Table("devices")]
    public class Device
    {
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("device_id")] public string DeviceId { get; set; }
        [Column("model")] public string Model { get; set; }
        [Column("manufacturer")] public string Manufacturer { get; set; }
        [Column("sys_version")] public string SystemVersion { get; set; }
        [Column("last_ip")] public string LastIp { get; set; }

        [Column("added_at")] public DateTime AddedAt { get; set; }
        [Column("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}