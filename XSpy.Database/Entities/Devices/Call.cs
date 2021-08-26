using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_calls")]
    public class Call : BaseDeviceEntity
    {
        [Column("number"), MaxLength(30)] public string Number { get; set; }
        [Column("name"), MaxLength(255)] public string Name { get; set; }
        [Column("duration"), MaxLength(10)] public string Duration { get; set; }
        [Column("device_date")] public DateTime Date { get; set; }
        [Column("hash"), MaxLength(32)] public string Hash { get; set; }
        [Column("call_type")] public CallType Type { get; set; }

    }
}