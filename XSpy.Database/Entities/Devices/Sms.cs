using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_sms")]
    public class Sms : BaseDeviceEntity
    {
        [Column("address"), MaxLength(30)] public string Address { get; set; }
        [Column("body"), MaxLength(180)] public string Name { get; set; }
        [Column("device_date"), MaxLength(30)] public string Date { get; set; }
        [Column("call_type")] public CallType Type { get; set; }

    }
}